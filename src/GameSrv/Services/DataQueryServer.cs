﻿using M2Server;
using NLog;
using System.Net;
using System.Net.Sockets;
using SystemModule;
using SystemModule.Packets.ServerPackets;
using SystemModule.SocketComponents.AsyncSocketClient;
using SystemModule.SocketComponents.Event;

namespace GameSrv.Services
{
    /// <summary>
    /// 玩家数据读写服务
    /// </summary>
    public class DataQueryServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ScoketClient _clientSocket;
        private byte[] ReceiveBuffer { get; set; }
        private int BuffLen { get; set; }
        private bool SocketWorking { get; set; }

        public DataQueryServer()
        {
            _clientSocket = new ScoketClient(new IPEndPoint(IPAddress.Parse(SystemShare.Config.sDBAddr), SystemShare.Config.nDBPort), 4096);
            _clientSocket.OnConnected += DataScoketConnected;
            _clientSocket.OnDisconnected += DataScoketDisconnected;
            _clientSocket.OnReceivedData += DataSocketRead;
            _clientSocket.OnError += DataSocketError;
            SocketWorking = false;
            ReceiveBuffer = new byte[10 * 2048];
        }

        public void Start()
        {
            _clientSocket.Connect();
        }

        public void Stop()
        {
            _clientSocket.Disconnect();
        }

        public bool IsConnected => _clientSocket.IsConnected;

        public void CheckConnected()
        {
            if (_clientSocket.IsConnected)
            {
                return;
            }
            if (_clientSocket.IsBusy)
            {
                return;
            }
            _clientSocket.Connect(SystemShare.Config.sDBAddr, SystemShare.Config.nDBPort);
        }

        public bool SendRequest<T>(int queryId, ServerRequestMessage message, T packet)
        {
            if (!_clientSocket.IsConnected)
            {
                return false;
            }
            var requestPacket = new ServerRequestData();
            requestPacket.QueryId = queryId;
            requestPacket.Message = EDCode.EncodeBuffer(SerializerUtil.Serialize(message));
            requestPacket.Packet = EDCode.EncodeBuffer(SerializerUtil.Serialize(packet));
            var signId = HUtil32.MakeLong((ushort)(queryId ^ 170), (ushort)(requestPacket.Message.Length + requestPacket.Packet.Length + ServerDataPacket.FixedHeaderLen));
            requestPacket.Sign = EDCode.EncodeBuffer(BitConverter.GetBytes(signId));
            SendMessage(SerializerUtil.Serialize(requestPacket));
            return true;
        }

        private void SendMessage(byte[] sendBuffer)
        {
            var serverMessage = new ServerDataPacket
            {
                PacketCode = Grobal2.PacketCode,
                PacketLen = (ushort)sendBuffer.Length
            };
            var dataBuff = SerializerUtil.Serialize(serverMessage);
            var data = new byte[ServerDataPacket.FixedHeaderLen + sendBuffer.Length];
            MemoryCopy.BlockCopy(dataBuff, 0, data, 0, data.Length);
            MemoryCopy.BlockCopy(sendBuffer, 0, data, dataBuff.Length, sendBuffer.Length);
            _clientSocket.Send(data);
        }

        private void DataScoketDisconnected(object sender, DSCClientConnectedEventArgs e)
        {
            _clientSocket.IsConnected = false;
            _logger.Error("数据库服务器[" + e.RemoteEndPoint + "]断开连接...");
        }

        private void DataScoketConnected(object sender, DSCClientConnectedEventArgs e)
        {
            _clientSocket.IsConnected = true;
            _logger.Info("数据库服务器[" + e.RemoteEndPoint + "]连接成功...");
        }

        private void DataSocketError(object sender, DSCClientErrorEventArgs e)
        {
            _clientSocket.IsConnected = false;
            switch (e.ErrorCode)
            {
                case SocketError.ConnectionRefused:
                    _logger.Error("数据库服务器[" + SystemShare.Config.sDBAddr + ":" + SystemShare.Config.nDBPort + "]拒绝链接...");
                    break;
                case SocketError.ConnectionReset:
                    _logger.Error("数据库服务器[" + SystemShare.Config.sDBAddr + ":" + SystemShare.Config.nDBPort + "]关闭连接...");
                    break;
                case SocketError.TimedOut:
                    _logger.Error("数据库服务器[" + SystemShare.Config.sDBAddr + ":" + SystemShare.Config.nDBPort + "]链接超时...");
                    break;
            }
        }

        private void DataSocketRead(object sender, DSCClientDataInEventArgs e)
        {
            HUtil32.EnterCriticalSection(M2Share.UserDBCriticalSection);
            try
            {
                var nMsgLen = e.BuffLen;
                var packetData = e.Buff;
                if (BuffLen > 0)
                {
                    MemoryCopy.BlockCopy(packetData, 0, ReceiveBuffer, BuffLen, packetData.Length);
                    ProcessServerPacket(ReceiveBuffer, BuffLen + nMsgLen);
                }
                else
                {
                    ProcessServerPacket(packetData, nMsgLen);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.UserDBCriticalSection);
            }
        }

        private void ProcessServerPacket(Span<byte> buff, int buffLen)
        {
            try
            {
                var srcOffset = 0;
                var nLen = buffLen;
                var dataBuff = buff;
                while (nLen >= ServerDataPacket.FixedHeaderLen)
                {
                    var packetHead = dataBuff[..ServerDataPacket.FixedHeaderLen];
                    var message = SerializerUtil.Deserialize<ServerDataPacket>(packetHead);
                    if (message.PacketCode != Grobal2.PacketCode)
                    {
                        srcOffset++;
                        dataBuff = dataBuff.Slice(srcOffset, ServerDataPacket.FixedHeaderLen);
                        nLen -= 1;
                        _logger.Debug($"解析封包出现异常封包，PacketLen:[{dataBuff.Length}] Offset:[{srcOffset}].");
                        continue;
                    }
                    var nCheckMsgLen = Math.Abs(message.PacketLen + ServerDataPacket.FixedHeaderLen);
                    if (nCheckMsgLen > nLen)
                    {
                        break;
                    }
                    SocketWorking = true;
                    var messageData = SerializerUtil.Deserialize<ServerRequestData>(dataBuff[ServerDataPacket.FixedHeaderLen..]);
                    ProcessServerData(messageData);
                    nLen -= nCheckMsgLen;
                    if (nLen <= 0)
                    {
                        break;
                    }
                    dataBuff = dataBuff.Slice(nCheckMsgLen, nLen);
                    BuffLen = nLen;
                    srcOffset = 0;
                    if (nLen < ServerDataPacket.FixedHeaderLen)
                    {
                        break;
                    }
                }
                if (nLen > 0)//有部分数据被处理,需要把剩下的数据拷贝到接收缓冲的头部
                {
                    MemoryCopy.BlockCopy(dataBuff, 0, ReceiveBuffer, 0, nLen);
                    BuffLen = nLen;
                }
                else
                {
                    BuffLen = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessServerData(ServerRequestData responsePacket)
        {
            try
            {
                if (!SocketWorking) return;
                if (responsePacket != null)
                {
                    var respCheckCode = responsePacket.QueryId;
                    var nLen = responsePacket.Message.Length + responsePacket.Packet.Length + ServerDataPacket.FixedHeaderLen;
                    if (nLen >= 12)
                    {
                        var queryId = HUtil32.MakeLong((ushort)(respCheckCode ^ 170), (ushort)nLen);
                        if (queryId <= 0 || responsePacket.Sign.Length <= 0)
                        {
                            SystemShare.Config.nLoadDBErrorCount++;
                            return;
                        }
                        var signBuff = EDCode.DecodeBuff(responsePacket.Sign);
                        if (queryId == BitConverter.ToInt16(signBuff))
                        {
                            CharacterDataService.Enqueue(respCheckCode, responsePacket);
                        }
                        else
                        {
                            SystemShare.Config.nLoadDBErrorCount++;
                        }
                    }
                }
                else
                {
                    _logger.Error("错误的封包数据");
                }
            }
            finally
            {
                SocketWorking = false;
            }
        }
    }
}