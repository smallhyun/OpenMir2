using System;
using System.Collections.Generic;
using SystemModule;
using SystemModule.Packages;
using SystemModule.Sockets;

namespace GameGate
{
    /// <summary>
    /// 用户会话封包处理
    /// </summary>
    public class UserClientSession
    {
        private GameSpeed _gameSpeed;
        private int nSpeedCount = 0;
        private int mSpeedCount = 0;
        /// <summary>
        /// 最高的人物身上所有装备+速度，默认6。
        /// </summary>
        private int nItemSpeed = 0;
        /// <summary>
        /// 玩家加速度装备因数，数值越小，封加速越严厉，默认60。
        /// </summary>
        private int DefItemSpeed;
        private int LastDirection = -1;
        private IList<TDelayMsg> _msgList;
        private GateConfig _gateConfig;
        private byte _handleLogin = 0;
        private bool _SpeedLimit;
        private int _sessionIdx;
        private TSessionInfo _session;
        public string sSendData = string.Empty;
        private bool m_fOverClientCount;
        private byte m_fHandleLogin;
        private byte[] m_xHWID;
        public bool m_fKickFlag = false;
        public bool m_fSpeedLimit = false;
        public long m_dwSessionID = 0;
        public int m_nSvrListIdx = 0;
        public int m_nSvrObject = 0;
        public int m_nChrStutas = 0;
        public int m_nItemSpeed = 0;
        public int m_nDefItemSpeed = 0;
        public int m_dwChatTick = 0;
        public int m_dwLastDirection = 0;
        public int m_dwEatTick = 0;
        public int m_dwHeroEatTick = 0;
        public int m_dwPickupTick = 0;
        public int m_dwMoveTick = 0;
        public int m_dwAttackTick = 0;
        public int m_dwSpellTick = 0;
        public int m_dwSitDownTick = 0;
        public int m_dwTurnTick = 0;
        public int m_dwButchTick = 0;
        public int m_dwDealTick = 0;
        public int m_dwOpenStoreTick = 0;
        public int m_dwWaringTick = 0;
        public int m_dwClientTimeOutTick = 0;
        public int m_SendCheckTick = 0;
        public TCheckStep m_Stat;
        public long m_FinishTick = 0;

        public UserClientSession(TSessionInfo session)
        {
            _gameSpeed = new GameSpeed();
            _gameSpeed.nErrorCount = 0; // 加速的累计值
            _gameSpeed.m_nHitSpeed = 0; // 装备加速
            _gameSpeed.dwSayMsgTick = HUtil32.GetTickCount(); // 发言时间
            _gameSpeed.dwAttackTick = HUtil32.GetTickCount(); // 攻击时间
            _gameSpeed.dwSpellTick = HUtil32.GetTickCount(); // 魔法时间
            _gameSpeed.dwWalkTick = HUtil32.GetTickCount(); // 走路时间
            _gameSpeed.dwRunTick = HUtil32.GetTickCount(); // 跑步时间
            _gameSpeed.dwTurnTick = HUtil32.GetTickCount(); // 转身时间
            _gameSpeed.dwButchTick = HUtil32.GetTickCount(); // 挖肉时间
            _gameSpeed.dwEatTick = HUtil32.GetTickCount(); // 吃药时间
            _gameSpeed.dwPickupTick = HUtil32.GetTickCount(); // 捡起时间
            _gameSpeed.dwRunWalkTick = HUtil32.GetTickCount(); // 移动时间
            _gameSpeed.dwFeiDnItemsTick = HUtil32.GetTickCount() - 10000; // 传送时间 15000 刚上来10秒内不能拣东西
            _gameSpeed.dwSupSpeederTick = HUtil32.GetTickCount(); // 变速齿轮时间
            _gameSpeed.dwSupSpeederCount = 0; // 变速齿轮累计
            _gameSpeed.dwSuperNeverTick = HUtil32.GetTickCount(); // 超级加速时间
            _gameSpeed.dwSuperNeverCount = 0; // 超级加速累计
            _gameSpeed.dwUserDoTick = 0; // 记录上一次操作
            _gameSpeed.dwContinueTick = 0; // 保存停顿操作时间
            _gameSpeed.dwConHitMaxCount = 0; // 带有攻击并发累计
            _gameSpeed.dwConSpellMaxCount = 0; // 带有魔法并发累计
            _gameSpeed.dwCombinationTick = 0; // 记录上一次移动方向
            _gameSpeed.dwCombinationCount = 0; // 智能攻击累计
            _gameSpeed.dwGameTick = HUtil32.GetTickCount(); // 在线时间
            _msgList = new List<TDelayMsg>();
            _session = session;
            _sessionIdx = session.SocketIdx;
            m_fOverClientCount = false;
        }

        public int SessionId => _sessionIdx;

        public GameSpeed GetGameSpeed()
        {
            return _gameSpeed;
        }

        public TSessionInfo GetSession()
        {
            return _session;
        }

        /// <summary>
        /// 处理客户端发送过来的封包
        /// todo 这里只需要封包就好 不需要在额外处理
        /// </summary>
        /// <param name="UserData"></param>
        public void HangdleUserPacket(TSendUserData UserData)
        {
            string sMsg = string.Empty;
            string sData = string.Empty;
            string sDefMsg = string.Empty;
            string sDataMsg = string.Empty;
            string sDataText = string.Empty;
            string sHumName = string.Empty;
            byte[] DataBuffer = null;
            int nOPacketIdx;
            int nPacketIdx;
            int nDataLen;
            int n14;
            TDefaultMessage DefMsg;
            TDefaultMessage Msg;
            try
            {
                n14 = 0;
                //nProcessMsgSize += UserData.sMsg.Length;
                if (_session.SocketIdx >= 0)
                {
                    if (UserData.nSocketHandle == _session.nSckHandle && _session.nPacketErrCount < 10)
                    {
                        if (_session.sSocData.Length > GateShare.MSGMAXLENGTH)
                        {
                            _session.sSocData = "";
                            _session.nPacketErrCount = 99;
                            UserData.sMsg = "";
                        }
                        sMsg = _session.sSocData + UserData.sMsg;
                        while (true)
                        {
                            sData = "";
                            sMsg = HUtil32.ArrestStringEx(sMsg, "#", "!", ref sData);
                            if (sData.Length > 2)
                            {
                                nPacketIdx = HUtil32.Str_ToInt(sData[0].ToString(), 99); // 将数据名第一位的序号取出
                                if (_session.nPacketIdx == nPacketIdx)
                                {
                                    // 如果序号重复则增加错误计数
                                    _session.nPacketErrCount++;
                                }
                                else
                                {
                                    nOPacketIdx = _session.nPacketIdx;
                                    _session.nPacketIdx = nPacketIdx;
                                    sData = sData.Substring(1, sData.Length - 1);
                                    nDataLen = sData.Length;
                                    if (nDataLen >= Grobal2.DEFBLOCKSIZE)
                                    {
                                        if (_session.boStartLogon)// 第一个人物登录数据包
                                        {
                                            // 第一个人物登录数据包   **1111/小小/6/120040918/0
                                            sDataText = EDcode.DeCodeString(sData);
                                            if ((sDataText[0] != '*') || (sDataText[1] != '*'))// 非法登陆
                                            {
                                                _session.nSckHandle = -1;
                                                _session.sSocData = "";
                                                _session.sSendData = "";
                                                _session.Socket.Close();
                                                _session.Socket = null;
                                                return;
                                            }
                                            //HandleLogin();
                                            sDataText = HUtil32.GetValidStr3(sDataText, ref sHumName, HUtil32.Backslash);
                                            sDataText = HUtil32.GetValidStr3(sDataText, ref _session.sUserName, HUtil32.Backslash); // 取角色名
                                            sDataText = "";
                                            sHumName = "";
                                            //nHumLogonMsgSize += sData.Length;
                                            _session.boStartLogon = false;
                                            sData = "#" + nPacketIdx + sData + "!";
                                            var sendBuff = HUtil32.GetBytes(sData);
                                            Send(Grobal2.GM_DATA, _session.SocketIdx, (int)_session.Socket.Handle, _session.nUserListIndex, sendBuff.Length, sendBuff);
                                        }
                                        else
                                        {
                                            // 普通数据包
                                            //nHumPlayMsgSize += sData.Length;
                                            if (nDataLen == Grobal2.DEFBLOCKSIZE)
                                            {
                                                sDefMsg = sData;
                                                sDataMsg = "";
                                            }
                                            else
                                            {
                                                sDefMsg = sData.Substring(0, Grobal2.DEFBLOCKSIZE);
                                                sDataMsg = sData.Substring(Grobal2.DEFBLOCKSIZE, sData.Length - Grobal2.DEFBLOCKSIZE);
                                            }
                                            DefMsg = EDcode.DecodeMessage(sDefMsg); // 检查数据
                                            if (GateShare.boStartSpeedCheck) //游戏速度控制
                                            {
                                                if (!_session.boSendAvailable)
                                                {
                                                    break;
                                                }
                                                var btSpeedControlMode = CheckDefMsg(DefMsg, _session, ref sDataMsg);
                                                switch (btSpeedControlMode)
                                                {
                                                    case 0:// 0停顿操作
                                                        _session.sSocData = "";// 清空所有当前动作
                                                        _gameSpeed.dwContinueTick = HUtil32.GetTickCount();// 保存停顿操作时间
                                                        SendWarnMsg(_session, GateShare.jWarningMsg, GateShare.btMsgFColorJ, GateShare.btMsgBColorJ);// 提示文字警告
                                                        continue;
                                                    case 1:// 1延迟处理
                                                        _session.bosendAvailableStart = true;
                                                        SendWarnMsg(_session, GateShare.yWarningMsg, GateShare.btMsgFColorY, GateShare.btMsgBColorY);// 提示文字警告
                                                        break;
                                                    case 2:// 2游戏掉线
                                                        mSpeedCount++;// 统计防御
                                                        sHumName = _session.sUserName;
                                                        if (!GateShare.GameSpeedList.ContainsKey(sHumName))
                                                        {
                                                            GateShare.GameSpeedList.TryAdd(sHumName, sHumName);
                                                        }
                                                        if (GateShare.boCheckBoxShowData)
                                                        {
                                                            GateShare.AddMainLogMsg("[超速提示]:" + sHumName + " 使用非法加速，已被T下线。", 3);
                                                        }
                                                        SendWarnMsg(_session, GateShare.sWarningMsg, GateShare.btMsgFColorS, GateShare.btMsgBColorS);// 提示文字警告
                                                        Send(Grobal2.GM_CLOSE, 0, (int)_session.Socket.Handle, 0, 0, null);// 发送给M2，通知T人
                                                        _session.nSckHandle = -1;
                                                        _session.sSocData = "";
                                                        _session.sSendData = "";
                                                        _session.Socket.Close();
                                                        _session.Socket = null;
                                                        _gameSpeed.nErrorCount = 0;// 清理累计
                                                        return;
                                                    case 3:// 3 执行脚本
                                                        nSpeedCount++;// 统计防御
                                                        sHumName = _session.sUserName;
                                                        if (!GateShare.GameSpeedList.ContainsKey(sHumName))
                                                        {
                                                            GateShare.GameSpeedList.TryAdd(sHumName, sHumName);
                                                        }
                                                        if (GateShare.boCheckBoxShowData)
                                                        {
                                                            GateShare.AddMainLogMsg("[超速提示]:" + sHumName + " 使用非法加速，已脚本处理。", 3);
                                                        }
                                                        SendWarnMsg(_session, GateShare.sWarningMsg, GateShare.btMsgFColorS, GateShare.btMsgBColorS);// 提示文字警告
                                                        _gameSpeed.nErrorCount = 0;// 清理累计
                                                        Msg = Grobal2.MakeDefaultMsg(Grobal2.CM_SAY, 0, 0, 0, 0);
                                                        sMsg = "#" + 0 + EDcode.EncodeMessage(Msg) + EDcode.EncodeString("@@加速处理") + "!" + sMsg;
                                                        //todo 应该直接抄送一份数据给M2，进行直接处理
                                                        break;
                                                    case 4:// 4 抛出封包
                                                        continue;
                                                    case 10:// 0 + 10 抛出封包      转身、挖肉、拣取  说话过滤
                                                        continue;
                                                    case 11:// 1 + 10 延迟处理      喝药
                                                        _session.bosendAvailableStart = true;
                                                        break;
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(sDataMsg))
                                            {
                                                DataBuffer = new byte[sDataMsg.Length + 12 + 1]; //GetMem(Buffer, sDataMsg.Length + 12 + 1);
                                                Buffer.BlockCopy(DefMsg.GetPacket(), 0, DataBuffer, 0, 12);//Move(DefMsg, Buffer, 12);
                                                var msgBuff = HUtil32.GetBytes(sDataMsg);
                                                Buffer.BlockCopy(msgBuff, 0, DataBuffer, 12, msgBuff.Length); //Move(sDataMsg[1], Buffer[12], sDataMsg.Length + 1);
                                                Send(Grobal2.GM_DATA, _session.SocketIdx, (int)_session.Socket.Handle, _session.nUserListIndex, DataBuffer.Length, DataBuffer);
                                            }
                                            else
                                            {
                                                DataBuffer = DefMsg.GetPacket();
                                                Send(Grobal2.GM_DATA, _session.SocketIdx, (int)_session.Socket.Handle, _session.nUserListIndex, 12, DataBuffer);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (n14 >= 1)
                                {
                                    sMsg = "";
                                }
                                else
                                {
                                    n14++;
                                }
                            }
                            if (HUtil32.TagCount(sMsg, '!') < 1)
                            {
                                break;
                            }
                        }
                        _session.sSocData = sMsg;
                    }
                    else
                    {
                        _session.sSocData = "";
                    }
                }
            }
            catch
            {
                if (_session.SocketIdx >= 0)
                {
                    sData = "[" + _session.sRemoteAddr + "]";
                }
                GateShare.AddMainLogMsg("[Exception] ProcessUserPacket" + sData, 1);
            }
        }

        /// <summary>
        /// 处理延时消息
        /// </summary>
        public void HandleDelayMsg()
        {
            if (GetDelayMsgCount() <= 0)
            {
                return;
            }
            TDelayMsg delayMsg = null;
            int dwCurrentTick = 0;
            while (GetDelayMsg(ref delayMsg))
            {
                if (delayMsg.nBufLen > 0)
                {
                    //todo 发送延时消息
                    
                    dwCurrentTick = HUtil32.GetTickCount();
                    switch (delayMsg.nCmd)
                    {
                        case Grobal2.CM_BUTCH:
                            _gameSpeed.dwButchTick = dwCurrentTick;
                            break;
                        case Grobal2.CM_SITDOWN:
                            _gameSpeed.dwSitDownTick = dwCurrentTick;
                            break;
                        case Grobal2.CM_TURN:
                            _gameSpeed.dwTurnTick = dwCurrentTick;
                            break;
                        case Grobal2.CM_WALK:
                        case Grobal2.CM_RUN:
                            _gameSpeed.dwMoveTick = dwCurrentTick;
                            _gameSpeed.dwSpellTick = dwCurrentTick - _gateConfig.m_nMoveNextSpellCompensate; //1200
                            if (_gameSpeed.dwAttackTick < dwCurrentTick - _gateConfig.m_nMoveNextAttackCompensate)
                            {
                                _gameSpeed.dwAttackTick = dwCurrentTick - _gateConfig.m_nMoveNextAttackCompensate; //900
                            }
                            LastDirection = delayMsg.nDir;
                            break;
                        case Grobal2.CM_HIT:
                        case Grobal2.CM_HEAVYHIT:
                        case Grobal2.CM_BIGHIT:
                        case Grobal2.CM_POWERHIT:
                        case Grobal2.CM_LONGHIT:
                        case Grobal2.CM_WIDEHIT:
                        case Grobal2.CM_CRSHIT:
                        case Grobal2.CM_FIREHIT:
                            if (_gameSpeed.dwAttackTick < dwCurrentTick)
                            {
                                _gameSpeed.dwAttackTick = dwCurrentTick;
                            }
                            if (_gateConfig.m_fItemSpeedCompensate)
                            {
                                _gameSpeed.dwMoveTick = dwCurrentTick - (_gateConfig.m_nAttackNextMoveCompensate + _gateConfig.m_nMaxItemSpeedRate * nItemSpeed);// 550
                                _gameSpeed.dwSpellTick = dwCurrentTick - (_gateConfig.m_nAttackNextSpellCompensate + _gateConfig.m_nMaxItemSpeedRate * nItemSpeed);// 1150
                            }
                            else
                            {
                                _gameSpeed.dwMoveTick = dwCurrentTick - _gateConfig.m_nAttackNextMoveCompensate; // 550
                                _gameSpeed.dwSpellTick = dwCurrentTick - _gateConfig.m_nAttackNextSpellCompensate;// 1150
                            }
                            LastDirection = delayMsg.nDir;
                            break;
                        case Grobal2.CM_SPELL:
                            _gameSpeed.dwSpellTick = dwCurrentTick;
                            int nNextMov = 0;
                            int nNextAtt = 0;
                            if (GateShare.Magic_Attack_Array[delayMsg.nMag])
                            {
                                nNextMov = _gateConfig.m_nSpellNextMoveCompensate;
                                nNextAtt = _gateConfig.m_nSpellNextAttackCompensate;
                            }
                            else
                            {
                                nNextMov = _gateConfig.m_nSpellNextMoveCompensate + 80;
                                nNextAtt = _gateConfig.m_nSpellNextAttackCompensate + 80;
                            }
                            _gameSpeed.dwMoveTick = dwCurrentTick - nNextMov;// 550
                            if (_gameSpeed.dwAttackTick < dwCurrentTick - nNextAtt)// 900
                            {
                                _gameSpeed.dwAttackTick = dwCurrentTick - nNextAtt;
                            }
                            LastDirection = delayMsg.nDir;
                            break;
                    }
                }
            }
        }

        private void PeekDelayMsg()
        {

        }

        /// <summary>
        /// 发送延时处理消息
        /// </summary>
        private void SendDelayMsg(int nMid, int nDir, int nIdx, int nLen, string pMsg, long dwDelay)
        {
            const int DELAY_BUFFER_LEN = 1024;
            if (nLen > 0 && nLen <= DELAY_BUFFER_LEN)
            {
                var pDelayMsg = new TDelayMsg();
                pDelayMsg.nMag = nMid;
                pDelayMsg.nDir = nDir;
                pDelayMsg.nCmd = nIdx;
                pDelayMsg.dwDelayTime = HUtil32.GetTickCount() + dwDelay;
                pDelayMsg.nBufLen = nLen;
                var bMsg = HUtil32.StringToByteAry(pMsg);
                pDelayMsg.pBuffer = bMsg;
                _msgList.Add(pDelayMsg);
            }
        }

        private void SendDelayMsg(int nMid, int nDir, int nIdx, int nLen, byte[] pMsg, long dwDelay)
        {
            const int DELAY_BUFFER_LEN = 1024;
            if (nLen > 0 && nLen <= DELAY_BUFFER_LEN)
            {
                var pDelayMsg = new TDelayMsg();
                pDelayMsg.nMag = nMid;
                pDelayMsg.nDir = nDir;
                pDelayMsg.nCmd = nIdx;
                pDelayMsg.dwDelayTime = HUtil32.GetTickCount() + dwDelay;
                pDelayMsg.nBufLen = nLen;
                pDelayMsg.pBuffer = pMsg;
                _msgList.Add(pDelayMsg);
            }
        }

        /// <summary>
        /// 获取延时消息
        /// </summary>
        private bool GetDelayMsg(ref TDelayMsg delayMsg)
        {
            var result = false;
            TDelayMsg _delayMsg = null;
            var count = 0;
            while (_msgList.Count > 0)
            {
                _delayMsg = _msgList[count];
                if (_delayMsg.dwDelayTime != 0 && HUtil32.GetTickCount() < _delayMsg.dwDelayTime)
                {
                    count++;
                    continue;
                }
                _msgList.RemoveAt(count);
                delayMsg.nMag = _delayMsg.nMag;
                delayMsg.nDir = _delayMsg.nDir;
                delayMsg.nCmd = _delayMsg.nCmd;
                delayMsg.nBufLen = _delayMsg.nBufLen;
                delayMsg.pBuffer = _delayMsg.pBuffer;
                //Move(pDelayMsg.pBuffer[0], pMsg.pBuffer[0], pMsg.nBufLen);
                _delayMsg = null;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 发送消息到客户端
        /// </summary>
        public void SeneMessage(string sMsg = "")
        {
            string sData;
            string sSendBlock;
            //nDeCodeMsgSize += UserData.sMsg.Length;
            sData = sSendData + sMsg;
            while (!string.IsNullOrEmpty(sData))
            {
                if (sData.Length > GateShare.nClientSendBlockSize)
                {
                    sSendBlock = sData.Substring(0, GateShare.nClientSendBlockSize);
                    sData = sData.Substring(GateShare.nClientSendBlockSize, sData.Length - GateShare.nClientSendBlockSize);
                }
                else
                {
                    sSendBlock = sData;
                    sData = "";
                }
                //检查延迟处理
                // if (!UserSession.bosendAvailableStart)
                // {
                //     UserSession.bosendAvailableStart = false;
                //     UserSession.boSendAvailable = false;
                //     UserSession.dwTimeOutTime = HUtil32.GetTickCount();
                // }
                if (!_session.boSendAvailable) //用户延迟处理
                {
                    if (HUtil32.GetTickCount() > _session.dwTimeOutTime)
                    {
                        _session.boSendAvailable = true;
                        _session.nCheckSendLength = 0;
                        GateShare.boSendHoldTimeOut = true;
                        GateShare.dwSendHoldTick = HUtil32.GetTickCount();
                    }
                }
                if (_session.boSendAvailable)
                {
                    if (_session.nCheckSendLength >= GateShare.SENDCHECKSIZE) //M2发送大于512字节封包加'*'
                    {
                        if (!_session.boSendCheck)
                        {
                            _session.boSendCheck = true;
                            sSendBlock = "*" + sSendBlock;
                        }
                        if (_session.nCheckSendLength >= GateShare.SENDCHECKSIZEMAX)
                        {
                            _session.boSendAvailable = false;
                            _session.dwTimeOutTime = HUtil32.GetTickCount() + GateShare.dwClientCheckTimeOut;
                        }
                    }
                    if (_session.Socket != null && _session.Socket.Connected)
                    {
                        //nSendBlockSize += sSendBlock.Length;
                        _session.Socket.SendText(sSendBlock);
                    }
                    _session.nCheckSendLength += sSendBlock.Length;
                }
                else
                {
                    sData = sSendBlock + sData; //延时处理消息 需要单独额外的处理
                    GateShare.AddMainLogMsg("延时处理消息:" + sData, 1);
                    break;
                }
            }
            sSendData = sData;
        }

        private void SendDefMessage(ushort wIdent, int nRecog, short nParam, short nTag, short nSeries, string sMsg)
        {

        }

        private void GetRealMsgId(ref int msgid)
        {
            var result = msgid;
            switch (msgid)
            {
                case 3014:
                    result = 3018; //CM_POWERHIT;
                    break;
                case 3003:
                    result = 3019; //CM_LONGHIT;
                    break;
                case 1007:
                    result = 1008; //CM_MAGICKEYCHANGE;
                    break;
                case 3017:
                    result = 3012; //CM_SITDOWN;
                    break;
                case 3016:
                    result = 3013; //CM_RUN;
                    break;
                case 3009:
                    result = 3010; //CM_TURN;
                    break;
                case 3018:
                    result = 3011; //CM_WALK;
                    break;
                case 3011:
                    result = 3016; //CM_BIGHIT;
                    break;
                case 3002:
                    result = 3017; //CM_SPELL;
                    break;
                case 3013:
                    result = 3014; //CM_HIT;
                    break;
                case 3012:
                    result = 3015; //CM_HEAVYHIT;
                    break;
                case 3010:
                    result = 3005; //CM_THROW;
                    break;
                case 1008:
                    result = 3003; //CM_SQUHIT;
                    break;
                case 3019:
                    result = 3002; //CM_PURSUEHIT;
                    break;
                case 1006:
                    result = 1007; //CM_BUTCH;
                    break;
                case 3015:
                    result = 1006; //CM_EAT;
                    break;
                case 3005:
                    result = 3009; //CM_HORSERUN;
                    break;

            }
        }

        private int GetDelayMsgCount()
        {
            return _msgList.Count;
        }

        private void SendKickMsg(int killType)
        {

        }

        private void HandleLogin(string loginData, int nLen, string Addr, ref bool success)
        {
            const int FIRST_PAKCET_MAX_LEN = 254;
            if (nLen < FIRST_PAKCET_MAX_LEN && nLen > 15)
            {
                if (loginData[0] != '*' || loginData[1] != '*')
                {
                    //if (g_pLogMgr.CheckLevel)
                    //{
                    //    g_pLogMgr.Add($"[HandleLogin] Kicked 1: {loginData}");
                    //}
                    success = false;
                    return;
                }
            }
            var sDataText = loginData;
            var sHumName = string.Empty;//人物名称
            var sAccount = string.Empty;//账号
            var szCert = string.Empty;
            var szClientVerNO = string.Empty;//客户端版本号
            var szCode = string.Empty;
            var szHarewareID = string.Empty;//硬件ID
            var sData = string.Empty;

            sDataText = HUtil32.GetValidStr3(sDataText, ref sAccount, HUtil32.Backslash);
            sDataText = HUtil32.GetValidStr3(sDataText, ref sHumName, HUtil32.Backslash);
            if ((sAccount.Length > 4) && (sAccount.Length <= 12) && (sHumName.Length > 2) && (sHumName.Length < 15))
            {
                sDataText = HUtil32.GetValidStr3(sDataText, ref szCert, HUtil32.Backslash);
                sDataText = HUtil32.GetValidStr3(sDataText, ref szClientVerNO, HUtil32.Backslash);
                sDataText = HUtil32.GetValidStr3(sDataText, ref szCode, HUtil32.Backslash);
                sDataText = HUtil32.GetValidStr3(sDataText, ref szHarewareID, HUtil32.Backslash);
                if (szCert.Length <= 0 || szCert.Length > 8)
                {
                    success = false;
                    return;
                }
                if (szClientVerNO.Length != 9)
                {
                    success = false;
                    return;
                }
                if (szCode.Length != 1)
                {
                    success = false;
                    return;
                }
                var userType = GateShare.PunishList.ContainsKey(sHumName);
                if (userType)
                {
                    _SpeedLimit = true;
                    GateShare.PunishList[sHumName] = this;
                }
                if (_gateConfig.m_fProcClientHWID)
                {
                    if (string.IsNullOrEmpty(szHarewareID) || (szHarewareID.Length > 256) || ((szHarewareID.Length % 2) != 0))
                    {
                        GateShare.AddMainLogMsg($"[HandleLogin] Kicked 3: {sHumName}", 1);
                        SendKickMsg(4);
                        return;
                    }
                    var Src = szHarewareID;
                    var Key = "openmir2";
                    var KeyLen = Key.Length;
                    var KeyPos = 0;
                    var OffSet = Convert.ToInt32("$" + Src.Substring(0, 2));
                    var SrcPos = 3;
                    var i = 0;
                    var SrcAsc = 0;
                    var TmpSrcAsc = 0;
                    var dest = new byte[1024];
                    var fMatch = false;
                    try
                    {
                        do
                        {
                            SrcAsc = Convert.ToInt32("$" + Src.Substring(SrcPos - 1, 2));
                            if (KeyPos < KeyLen)
                            {
                                KeyPos = KeyPos + 1;
                            }
                            else
                            {
                                KeyPos = 1;
                            }
                            TmpSrcAsc = SrcAsc ^ (int)(Key[KeyPos]);
                            if (TmpSrcAsc <= OffSet)
                            {
                                TmpSrcAsc = 255 + TmpSrcAsc - OffSet;
                            }
                            else
                            {
                                TmpSrcAsc = TmpSrcAsc - OffSet;
                            }
                            dest[i] = (byte)(TmpSrcAsc);
                            i++;
                            OffSet = SrcAsc;
                            SrcPos = SrcPos + 2;
                        } while (!(SrcPos >= Src.Length));
                    }
                    catch (Exception e)
                    {
                        fMatch = true;
                    }
                    if (fMatch)
                    {
                        GateShare.AddMainLogMsg($"[HandleLogin] Kicked 5: {sHumName}", 1);
                        SendKickMsg(4);
                        return;
                    }
                    THardwareHeader pHardwareHeader = new THardwareHeader(dest);
                    //todo session会话里面需要存用户ip
                    GateShare.AddMainLogMsg(string.Format("HWID: {0}  {1}  {2}", MD5.MD5Print(pHardwareHeader.xMd5Digest), sHumName.Trim(), "ip"), 1);
                    if (pHardwareHeader.dwMagicCode == 0x13F13F13)
                    {
                        if (MD5.MD5Match(MD5.g_MD5EmptyDigest, pHardwareHeader.xMd5Digest))
                        {
                            // if (LogManager.Units.LogManager.g_pLogMgr.CheckLevel(10))
                            // {
                            //     LogManager.Units.LogManager.g_pLogMgr.Add(Format("[HandleLogin] Kicked 6: %s", new string[] {szCharName}));
                            // }
                            SendKickMsg(4);
                            return;
                        }
                        m_xHWID = pHardwareHeader.xMd5Digest;
                        if (Filter.g_HWIDFilter.IsFilter(m_xHWID, ref m_fOverClientCount))
                        {
                            // if (LogManager.Units.LogManager.g_pLogMgr.CheckLevel(10))
                            // {
                            //     LogManager.Units.LogManager.g_pLogMgr.Add(Format("[HandleLogin] Kicked 7: %s", new string[] {szCharName}));
                            // }
                            if (m_fOverClientCount)
                            {
                                SendKickMsg(5);
                            }
                            else
                            {
                                SendKickMsg(6);
                            }
                            return;
                        }
                    }
                    else
                    {
                        // if (LogManager.Units.LogManager.g_pLogMgr.CheckLevel(10))
                        // {
                        //     LogManager.Units.LogManager.g_pLogMgr.Add(Format("[HandleLogin] Kicked 8: %s", new string[] {szCharName}));
                        // }
                        SendKickMsg(4);
                        return;
                    }
                    //FillChar(pszLoginPacket, sizeof(pszLoginPacket), 0);
                    var pszLoginPacket = new byte[1047];
                    var szTemp = string.Format("**{0}/{1}/{2}/{3}/{4}/{5}", new[] { sAccount, sHumName, szCert, szClientVerNO, szCode, MD5.MD5Print(m_xHWID) });
                    // #0.........!
                    var tempBuff = HUtil32.GetBytes(szTemp);
                    Buffer.BlockCopy(tempBuff, 0, pszLoginPacket, 2, tempBuff.Length);
                    //var Len = Misc.EncodeBuf(szTemp[0], szTemp.Length, pszLoginPacket[2]);
                    pszLoginPacket[0] = (byte)'#';
                    pszLoginPacket[1] = (byte)'0';
                    pszLoginPacket[tempBuff.Length + 2] = (byte)'!';
                    m_fHandleLogin = 2;
                    SendFirstPack(pszLoginPacket);
                }
                else
                {
                    GateShare.AddMainLogMsg($"[HandleLogin] Kicked 2: {Addr}", 1);
                    success = false;
                }
            }
            else
            {
                GateShare.AddMainLogMsg($"[HandleLogin] Kicked 2: {loginData}", 1);
                success = false;
            }
            //sDataText = HUtil32.GetValidStr3(sDataText, ref _session.sUserName, HUtil32.Backslash); // 取角色名
            //sDataText = "";
            //sHumName = "";
            //nHumLogonMsgSize += sData.Length;
            //_session.boStartLogon = false;
            //sData = "#" + nPacketIdx + sData + "!";
            //var sendBuff = HUtil32.GetBytes(sData);
            //Send(Grobal2.GM_DATA, UserData.nSocketIdx, (int)UserData.UserClient.SessionArray[UserData.nSocketIdx].Socket.Handle,
            //    UserData.UserClient.SessionArray[UserData.nSocketIdx].nUserListIndex, sendBuff.Length, sendBuff);
        }

        /// <summary>
        /// 发送消息到GameSvr
        /// </summary>
        /// <param name="packet"></param>
        private void SendFirstPack(byte[] packet)
        {
            //Send(Grobal2.GM_DATA, _session.SocketIdx, (int)_session.Socket.Handle, _session.nUserListIndex, packet.Length, packet);
            // var tempBuff = new byte[20 + packet.Length];
            // var GateMsg = new TMsgHeader();
            // GateMsg.dwCode = Grobal2.RUNGATECODE;
            // GateMsg.nSocket = nSocket;
            // GateMsg.wGSocketIdx = wSocketIndex;
            // GateMsg.wIdent = Grobal2.GM_DATA;
            // GateMsg.wUserListIndex = nUserListIndex;
            // GateMsg.nLength = tempBuff.Length;
            // var sendBuffer = GateMsg.GetPacket();
            // Buffer.BlockCopy(sendBuffer, 0, tempBuff, 0, sendBuffer.Length);
            // Buffer.BlockCopy(packet, 0, tempBuff, sendBuffer.Length, packet.Length);
            // SendDelayMsg(0, 0, 0, tempBuff.Length, tempBuff, 1);
        }

        private void SendSysMsg(string szMsg)
        {
            // TCmdPack Cmd;
            // char[] TempBuf = new char[1023 + 1];
            // char[] SendBuf = new char[1023 + 1];
            // if ((m_tLastGameSvr == null) || !m_tLastGameSvr.Active)
            // {
            //     return;
            // }
            // Cmd.UID = m_nSvrObject;
            // Cmd.Cmd = Grobal2.SM_SYSMESSAGE;
            // Cmd.X = MakeWord(0xFF, 0xF9);
            // Cmd.Y = 0;
            // Cmd.Direct = 0;
            // SendBuf[0] = "#";
            // Move(Cmd, TempBuf[1], sizeof(TCmdPack));
            // Move(szMsg[1], TempBuf[13], szMsg.Length);
            // var iLen = sizeof(TCmdPack) + szMsg.Length;
            // iLen = Misc.Units.Misc.EncodeBuf(((int)TempBuf[1]), iLen, ((int)SendBuf[1]));
            // SendBuf[iLen + 1] = "!";
            // m_tIOCPSender.SendData(m_pOverlapSend, SendBuf[0], iLen + 2);
        }

        private int CheckDefMsg(TDefaultMessage DefMsg, TSessionInfo SessionInfo, ref string sMsg)
        {
            int result = -1;
            int NextHitTime;
            int LevelFastTime;
            int nMsgCount;
            string sDataText;
            string sHumName = string.Empty;
            try
            {
                TDefaultMessage? message = DefMsg;
                if (message == null)
                {
                    result = 2;
                    return result;
                }
                if ((SessionInfo == null))
                {
                    result = 2;
                    return result;
                }
                if ((SessionInfo.Socket == null))
                {
                    result = 2;
                    return result;
                }
                switch (DefMsg.Ident)
                {
                    case Grobal2.CM_WALK:
                        // 走路
                        // ------------------------------------
                        // 原理:外挂利用挖地.自身不显示下蹲动作,而达到比正常玩家快1步.
                        // 每次保存上一次动作,判断挖地操作后,少下蹲动作,作为挖地暗杀处理.
                        // ------------------------------------
                        if (GateShare.boDarkHitCheck)
                        {
                            // 封挖地暗杀
                            if (_gameSpeed.dwUserDoTick == Grobal2.CM_BUTCH)
                            {
                                result = 2;// 返回掉线处理
                                return result;
                            }
                        }
                        // ------------------------------------
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000) // 停顿操作后3秒内数据全部抛出
                        {
                            result = 4;// 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        // 原理：智能攻击，十字走位等，来回自动跑位0、2、4、6方向
                        // 只要判断重复以上移动动作超过10次以上，并判断为智能攻击。
                        // 重复1个动作只累加1次，如一直一个方向移动需要排除掉。
                        // ------------------------------------
                        if (GateShare.boCombinationCheck)
                        {
                            // 封掉组合攻击
                            if ((_gameSpeed.dwCombinationTick != DefMsg.Tag))
                            {
                                if ((DefMsg.Tag == 1) || (DefMsg.Tag == 3) || (DefMsg.Tag == 5) || (DefMsg.Tag == 7))
                                {
                                    if (_gameSpeed.dwCombinationCount > 10)
                                    {
                                        result = 2;// 返回掉线处理
                                        return result;
                                    }
                                    _gameSpeed.dwCombinationCount++;// 智能攻击累计
                                }
                                else
                                {
                                    _gameSpeed.dwCombinationCount = 0;// 清零
                                }
                            }
                            else
                            {
                                if (_gameSpeed.dwCombinationCount >= 1)
                                {
                                    _gameSpeed.dwCombinationCount -= 1; // 一个方向走减少累计
                                }
                            }
                            _gameSpeed.dwCombinationTick = DefMsg.Tag;// 记录移动方向
                        }
                        // ------------------------------------
                        if (GateShare.boStartWalkCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwWalkTick) < GateShare.dwSpinEditWalkTime)
                            {
                                _gameSpeed.nErrorCount += GateShare.nIncErrorCount;// 每次加速的累加值
                                if (_gameSpeed.nErrorCount >= GateShare.nSpinEditWalkCount) // 50
                                {
                                    if (SessionInfo.boSendAvailable)
                                    {
                                        SessionInfo.dwClientCheckTimeOut = 500; // 延迟间隔
                                    }
                                    else
                                    {
                                        SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditWalkTime; // 延迟间隔
                                    }
                                    _gameSpeed.dwSuperNeverTick = HUtil32.GetTickCount();// 保存超级加速时间
                                    _gameSpeed.nErrorCount = 0;// 清除累计值
                                    result = GateShare.dwComboBoxWalkCheck;// 返回走路加速处理
                                }
                            }
                            else
                            {
                                if (_gameSpeed.nErrorCount >= GateShare.nDecErrorCount)
                                {
                                    _gameSpeed.nErrorCount -= GateShare.nDecErrorCount;// 正常动作的减少值
                                }
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[走路间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwWalkTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);
                            }
                            // 封包显示
                            _gameSpeed.dwWalkTick = HUtil32.GetTickCount();// 保存走路时间
                        }
                        _gameSpeed.dwRunWalkTick = HUtil32.GetTickCount();// 保存移动时间
                        _gameSpeed.dwGameTick = HUtil32.GetTickCount();
                        break;
                    case Grobal2.CM_RUN:// 在线时间
                        // 跑步
                        // ------------------------------------
                        // 原理:外挂利用挖地.自身不显示下蹲动作,而达到比正常玩家快1步.
                        // 每次保存上一次动作,判断挖地操作后,少下蹲动作,作为挖地暗杀处理.
                        // ------------------------------------
                        if (GateShare.boDarkHitCheck) // 封挖地暗杀
                        {
                            if (_gameSpeed.dwUserDoTick == Grobal2.CM_BUTCH)
                            {
                                result = 2;// 返回掉线处理
                                return result;
                            }
                        }
                        // ------------------------------------
                        // 原理：返回移动加速处理 或 转向加速处理 后
                        // 再指定一段时间内 再次接收2次或2次以上跑步动作
                        // 将判断为外挂强行破解客户端程序，所谓超级不卡。
                        // ------------------------------------
                        if (GateShare.boSuperNeverCheck) // 封超级加速
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwSuperNeverTick) < HUtil32._MAX(500, GateShare.dwSpinEditRunTime))
                            {
                                if (_gameSpeed.dwSuperNeverCount >= 2)
                                {
                                    _gameSpeed.dwSuperNeverCount = 0;// 清零
                                    result = 2;// 返回掉线处理
                                    return result;
                                }
                                else
                                {
                                    _gameSpeed.dwSuperNeverCount++;// 超级加速累计
                                }
                            }
                        }
                        // ------------------------------------
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000)// 停顿操作后3秒内数据全部抛出
                        {
                            result = 4;// 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        // 原理：智能攻击，十字走位等，来回自动跑位0、2、4、6方向
                        // 只要判断重复以上移动动作超过10次以上，并判断为智能攻击。
                        // 重复1个动作只累加1次，如一直一个方向移动需要排除掉。
                        // ------------------------------------
                        if (GateShare.boCombinationCheck)
                        {
                            // 封掉组合攻击
                            if ((_gameSpeed.dwCombinationTick != DefMsg.Tag))
                            {
                                if ((DefMsg.Tag == 0) || (DefMsg.Tag == 2) || (DefMsg.Tag == 4) || (DefMsg.Tag == 6))
                                {
                                    if (_gameSpeed.dwCombinationCount > 10)
                                    {
                                        result = 2;// 返回掉线处理
                                        return result;
                                    }
                                    _gameSpeed.dwCombinationCount++;// 智能攻击累计
                                }
                                else
                                {
                                    _gameSpeed.dwCombinationCount = 0;// 清零
                                }
                            }
                            else
                            {
                                if (_gameSpeed.dwCombinationCount >= 1)
                                {
                                    _gameSpeed.dwCombinationCount -= 1;// 一个方向走减少累计
                                }
                            }
                            _gameSpeed.dwCombinationTick = DefMsg.Tag;// 记录移动方向
                        }
                        // ------------------------------------
                        if (GateShare.boStartRunCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwRunTick) < GateShare.dwSpinEditRunTime)
                            {
                                _gameSpeed.nErrorCount += GateShare.nIncErrorCount;// 每次加速的累加值
                                // 50
                                if (_gameSpeed.nErrorCount >= GateShare.nSpinEditRunCount)
                                {
                                    if (SessionInfo.boSendAvailable)
                                    {
                                        SessionInfo.dwClientCheckTimeOut = 500;// 延迟间隔
                                    }
                                    else
                                    {
                                        SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditRunTime;// 延迟间隔
                                    }
                                    _gameSpeed.dwSuperNeverTick = HUtil32.GetTickCount();// 保存超级加速时间
                                    _gameSpeed.nErrorCount = 0;// 清除累计值
                                    result = GateShare.dwComboBoxRunCheck;// 返回跑步加速处理
                                }
                            }
                            else
                            {
                                if (_gameSpeed.nErrorCount >= GateShare.nDecErrorCount)
                                {
                                    _gameSpeed.nErrorCount -= GateShare.nDecErrorCount; // 正常动作的减少值
                                }
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[跑步间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwRunTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);// 封包显示
                            }
                            _gameSpeed.dwRunTick = HUtil32.GetTickCount();// 保存跑步时间
                        }
                        _gameSpeed.dwRunWalkTick = HUtil32.GetTickCount();// 保存移动时间
                        _gameSpeed.dwGameTick = HUtil32.GetTickCount();
                        break;
                    //case Grobal2.CM_3035:// 在线时间
                    //    if (GateShare.boWalk3caseCheck) // 一步三格
                    //    {
                    //        result = 2;// 返回掉线处理
                    //        return result;
                    //    }
                    //    break;
                    //case Grobal2.CM_1099:// 过非法移动
                    //    if (GateShare.boSuperNeverCheck) // 封掉超级加速
                    //    {
                    //        result = 2;// 返回掉线处理
                    //        return result;
                    //    }
                    //    break;
                    case Grobal2.CM_HIT:
                    case Grobal2.CM_HEAVYHIT:
                    case Grobal2.CM_BIGHIT:
                    case Grobal2.CM_POWERHIT:
                    case Grobal2.CM_LONGHIT:
                    case Grobal2.CM_WIDEHIT:
                    case Grobal2.CM_FIREHIT:
                        // 霜月
                        // 攻击
                        // ------------------------------------
                        // 原理:外挂利用挖地.自身不显示下蹲动作,而达到比正常玩家快1步.
                        // 每次保存上一次动作,判断挖地操作后,少下蹲动作,作为挖地暗杀处理.
                        // ------------------------------------
                        if (GateShare.boDarkHitCheck) // 封挖地暗杀
                        {
                            if (_gameSpeed.dwUserDoTick == Grobal2.CM_BUTCH)
                            {
                                result = 2;// 返回掉线处理
                                return result;
                            }
                        }
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000)// 停顿操作后3秒内数据全部抛出
                        {
                            result = 4; // 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        // 原理:判断大于10条封包时，含有攻击操作，连续触犯2次，当秒杀处理。
                        // ------------------------------------
                        if (GateShare.boStartConHitMaxCheck)// 带有攻击并发控制
                        {
                            nMsgCount = HUtil32.TagCount(SessionInfo.sSocData, '!');
                            if (nMsgCount > GateShare.dwSpinEditConHitMaxTime)
                            {
                                if (_gameSpeed.dwConHitMaxCount > 1)
                                {
                                    _gameSpeed.dwConHitMaxCount = 0;// 清零
                                    result = GateShare.dwComboBoxConHitMaxCheck;// 返回带有攻击并发处理
                                    return result;
                                }
                                else
                                {
                                    _gameSpeed.dwConHitMaxCount++;// 带有攻击并发累计
                                }
                            }
                            else
                            {
                                _gameSpeed.dwConHitMaxCount = 0;// 清零
                            }
                        }
                        // ------------------------------------
                        // 原理:保存每次走路和跑步时间,判断移动后攻击之间间隔.
                        // ------------------------------------
                        if (GateShare.boStartRunhitCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwRunWalkTick) < GateShare.dwSpinEditRunhitTime) // 移动攻击
                            {
                                SessionInfo.dwClientCheckTimeOut = 5000;// 延迟间隔
                                result = GateShare.dwComboBoxRunhitCheck;// 返回移动攻击加速处理
                                return result;// 到这里停止下面攻击检测
                            }
                        }
                        // ------------------------------------
                        if (GateShare.boStartHitCheck)
                        {
                            LevelFastTime = HUtil32._MIN(430, _gameSpeed.m_nHitSpeed * GateShare.nItemSpeedCount);// 60
                            NextHitTime = HUtil32._MAX(200, GateShare.dwSpinEditHitTime - LevelFastTime);
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwAttackTick) < NextHitTime)
                            {
                                _gameSpeed.nErrorCount += GateShare.nIncErrorCount;// 每次加速的累加值
                                if (_gameSpeed.nErrorCount >= GateShare.nSpinEditHitCount)// 50
                                {
                                    if (SessionInfo.boSendAvailable)// 延迟间隔
                                    {
                                        SessionInfo.dwClientCheckTimeOut = 3000;
                                    }
                                    else
                                    {
                                        SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditHitTime; // 延迟间隔
                                    }
                                    _gameSpeed.nErrorCount = 0;// 清除累计值
                                    result = GateShare.dwComboBoxHitCheck;// 返回攻击加速处理
                                }
                            }
                            else
                            {
                                if (_gameSpeed.nErrorCount >= GateShare.nDecErrorCount)
                                {
                                    _gameSpeed.nErrorCount -= GateShare.nDecErrorCount;// 正常动作的减少值
                                }
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[攻击间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwAttackTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);// 封包显示
                            }
                            _gameSpeed.dwAttackTick = HUtil32.GetTickCount();// 保存攻击时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount();// 在线时间
                        }
                        break;
                    case Grobal2.CM_3037:
                        // ------------------------------------
                        // 原理:外挂利用挖地.自身不显示下蹲动作,而达到比正常玩家快1步.
                        // 每次保存上一次动作,判断挖地操作后,少下蹲动作,作为挖地暗杀处理.
                        // ------------------------------------
                        if (GateShare.boDarkHitCheck) // 封挖地暗杀
                        {
                            if (_gameSpeed.dwUserDoTick == Grobal2.CM_BUTCH)
                            {
                                result = 2;// 返回掉线处理
                                return result;
                            }
                        }
                        // ------------------------------------
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000) // 停顿操作后3秒内数据全部抛出
                        {
                            result = 4;// 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        // 原理:判断大于10条封包时，含有攻击操作，连续触犯2次，当秒杀处理。
                        // ------------------------------------
                        if (GateShare.boStartConHitMaxCheck)
                        {
                            // 带有攻击并发控制
                            nMsgCount = HUtil32.TagCount(SessionInfo.sSocData, '!');
                            if (nMsgCount > GateShare.dwSpinEditConHitMaxTime)
                            {
                                if (_gameSpeed.dwConHitMaxCount > 1)
                                {
                                    _gameSpeed.dwConHitMaxCount = 0;// 清零
                                    result = GateShare.dwComboBoxConHitMaxCheck;// 返回带有攻击并发处理
                                    return result;
                                }
                                else
                                {
                                    _gameSpeed.dwConHitMaxCount++;// 带有攻击并发累计
                                }
                            }
                            else
                            {
                                _gameSpeed.dwConHitMaxCount = 0;// 清零
                            }
                        }
                        // ------------------------------------
                        // 原理:保存每次走路和跑步时间,判断移动后攻击之间间隔.
                        // ------------------------------------
                        if (GateShare.boStartRunhitCheck) // 移动攻击
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwRunWalkTick) < GateShare.dwSpinEditRunhitTime)
                            {
                                SessionInfo.dwClientCheckTimeOut = 5000;// 延迟间隔
                                result = GateShare.dwComboBoxRunhitCheck;// 返回移动攻击加速处理
                                return result;// 到这里停止下面攻击检测
                            }
                        }
                        // ------------------------------------
                        if (GateShare.boAfterHitCheck)
                        {
                            LevelFastTime = HUtil32._MIN(430, _gameSpeed.m_nHitSpeed * GateShare.nItemSpeedCount); // 60
                            NextHitTime = HUtil32._MAX(200, GateShare.dwSpinEditHitTime - LevelFastTime);
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwAttackTick) < NextHitTime)
                            {
                                _gameSpeed.nErrorCount += GateShare.nIncErrorCount;// 每次加速的累加值
                                if (_gameSpeed.nErrorCount >= GateShare.nSpinEditHitCount) // 50
                                {
                                    if (SessionInfo.boSendAvailable)
                                    {
                                        SessionInfo.dwClientCheckTimeOut = 3000; // 延迟间隔
                                    }
                                    else
                                    {
                                        SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditHitTime; // 延迟间隔
                                    }
                                    _gameSpeed.nErrorCount = 0;// 清除累计值
                                    result = GateShare.dwComboBoxHitCheck;// 返回攻击加速处理
                                }
                            }
                            else
                            {
                                if (_gameSpeed.nErrorCount >= GateShare.nDecErrorCount)
                                {
                                    _gameSpeed.nErrorCount -= GateShare.nDecErrorCount; // 正常动作的减少值
                                }
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[攻击间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwAttackTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);// 封包显示
                            }
                            _gameSpeed.dwAttackTick = HUtil32.GetTickCount();// 保存攻击时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount();// 在线时间
                        }
                        break;
                    case Grobal2.CM_SPELL: // 魔法
                        // ------------------------------------
                        // 原理:外挂利用挖地.自身不显示下蹲动作,而达到比正常玩家快1步.
                        // 每次保存上一次动作,判断挖地操作后,少下蹲动作,作为挖地暗杀处理.
                        // ------------------------------------
                        if (GateShare.boDarkHitCheck)
                        {
                            // 封挖地暗杀
                            if (_gameSpeed.dwUserDoTick == Grobal2.CM_BUTCH)
                            {
                                result = 2;// 返回掉线处理
                                return result;
                            }
                        }
                        // ------------------------------------
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000) // 停顿操作后3秒内数据全部抛出
                        {
                            result = 4;// 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        // 原理:判断大于10条封包时，含有魔法操作，连续触犯2次，当秒杀处理。
                        // ------------------------------------
                        if (GateShare.boStartConSpellMaxCheck)
                        {
                            // 带有魔法并发控制
                            nMsgCount = HUtil32.TagCount(SessionInfo.sSocData, '!');
                            if (nMsgCount > GateShare.dwSpinEditConSpellMaxTime)
                            {
                                if (_gameSpeed.dwConSpellMaxCount > 1)
                                {
                                    _gameSpeed.dwConSpellMaxCount = 0;// 清零
                                    result = GateShare.dwComboBoxConSpellMaxCheck;// 返回带有攻击并发处理
                                    return result;
                                }
                                else
                                {
                                    _gameSpeed.dwConSpellMaxCount++;// 带有魔法并发累计
                                }
                            }
                            else
                            {
                                _gameSpeed.dwConSpellMaxCount = 0;// 清零
                            }
                        }
                        // ------------------------------------
                        // 原理:保存每次走路和跑步时间,判断移动后魔法之间间隔.
                        // ------------------------------------
                        if (GateShare.boStartRunhitCheck)// 移动魔法
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwRunWalkTick) < GateShare.dwSpinEditRunspellTime)
                            {
                                SessionInfo.dwClientCheckTimeOut = 5000;// 延迟间隔
                                result = GateShare.dwComboBoxRunspellCheck;// 返回移动魔法加速处理
                                return result;// 到这里停止下面攻击检测
                            }
                        }
                        switch (DefMsg.Tag)
                        {
                            case Grobal2.SKILL_FIREBALL:
                            case Grobal2.SKILL_HEALLING:
                            case Grobal2.SKILL_FIREBALL2:
                            case Grobal2.SKILL_AMYOUNSUL:
                            case Grobal2.SKILL_FIREWIND:
                            case Grobal2.SKILL_FIRE:
                            case Grobal2.SKILL_SHOOTLIGHTEN:
                            case Grobal2.SKILL_LIGHTENING:
                            case Grobal2.SKILL_FIRECHARM:
                            case Grobal2.SKILL_HANGMAJINBUB:
                            case Grobal2.SKILL_DEJIWONHO:
                            case Grobal2.SKILL_HOLYSHIELD:
                            case Grobal2.SKILL_SKELLETON:
                            case Grobal2.SKILL_CLOAK:
                            case Grobal2.SKILL_BIGCLOAK:
                            case Grobal2.SKILL_TAMMING:
                            case Grobal2.SKILL_SPACEMOVE:
                            case Grobal2.SKILL_EARTHFIRE:
                            case Grobal2.SKILL_FIREBOOM:
                            case Grobal2.SKILL_LIGHTFLOWER:
                            case Grobal2.SKILL_MOOTEBO:
                            case Grobal2.SKILL_SHOWHP:
                            case Grobal2.SKILL_BIGHEALLING:
                            case Grobal2.SKILL_SINSU:
                            case Grobal2.SKILL_SHIELD:
                            case Grobal2.SKILL_KILLUNDEAD:
                            case Grobal2.SKILL_SNOWWIND:
                            case Grobal2.SKILL_UNAMYOUNSUL:
                            case Grobal2.SKILL_WINDTEBO:
                            case Grobal2.SKILL_MABE:
                            case Grobal2.SKILL_GROUPLIGHTENING:
                            case Grobal2.SKILL_GROUPAMYOUNSUL:
                            case Grobal2.SKILL_GROUPDEDING:
                            case Grobal2.SKILL_44:
                            case Grobal2.SKILL_45:
                            case Grobal2.SKILL_46:
                            case Grobal2.SKILL_47:
                            case Grobal2.SKILL_49:
                            case Grobal2.SKILL_51:
                            case Grobal2.SKILL_52:
                            case Grobal2.SKILL_53:
                            case Grobal2.SKILL_54:
                            case Grobal2.SKILL_55:
                            case Grobal2.SKILL_57:
                            case Grobal2.SKILL_58:
                            case Grobal2.SKILL_59:
                                if (GateShare.boStartSpellCheck)
                                {
                                    if ((HUtil32.GetTickCount() - _gameSpeed.dwSpellTick) < GateShare.dwSpinEditSpellTime)
                                    {
                                        _gameSpeed.nErrorCount += GateShare.nIncErrorCount; // 每次加速的累加值
                                        if (_gameSpeed.nErrorCount >= GateShare.nSpinEditSpellCount)// 50
                                        {
                                            if (SessionInfo.boSendAvailable)
                                            {
                                                SessionInfo.dwClientCheckTimeOut = 3000;// 延迟间隔
                                            }
                                            else
                                            {
                                                SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditSpellTime;// 延迟间隔
                                            }
                                            _gameSpeed.nErrorCount = 0; // 清除累计值
                                            result = GateShare.dwComboBoxSpellCheck; // 返回魔法加速处理
                                        }
                                    }
                                    else
                                    {
                                        if (_gameSpeed.nErrorCount >= GateShare.nDecErrorCount)
                                        {
                                            _gameSpeed.nErrorCount -= GateShare.nDecErrorCount;// 正常动作的减少值
                                        }
                                    }
                                    if (SessionInfo.sUserName == GateShare.boMsgUserName)
                                    {
                                        GateShare.AddMainLogMsg("[魔法间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwSpellTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3); // 封包显示
                                    }
                                    _gameSpeed.dwSpellTick = HUtil32.GetTickCount(); // 保存魔法时间
                                    _gameSpeed.dwGameTick = HUtil32.GetTickCount(); // 在线时间
                                }
                                break;
                        }
                        break;
                    case Grobal2.CM_TURN: // 转身     （只有停顿和延迟处理）
                        // ------------------------------------
                        // 原理:外挂利用挖地.自身不显示下蹲动作,而达到比正常玩家快1步.
                        // 每次保存上一次动作,判断挖地操作后,少下蹲动作,作为挖地暗杀处理.
                        // ------------------------------------
                        if (GateShare.boDarkHitCheck) // 封挖地暗杀
                        {
                            if (_gameSpeed.dwUserDoTick == Grobal2.CM_BUTCH)
                            {
                                result = 2;// 返回掉线处理
                                return result;
                            }
                        }
                        // ------------------------------------
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000) // 停顿操作后3秒内数据全部抛出
                        {
                            result = 4;// 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        if (GateShare.boStartTurnCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwTurnTick) < GateShare.dwSpinEditTurnTime)
                            {
                                if (SessionInfo.boSendAvailable)
                                {
                                    SessionInfo.dwClientCheckTimeOut = 200;// 延迟间隔
                                }
                                else
                                {
                                    SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditTurnTime;// 延迟间隔
                                }
                                _gameSpeed.dwSuperNeverTick = HUtil32.GetTickCount() - 300;// 保存超级加速时间
                                result = GateShare.dwComboBoxTurnCheck + 10;// 返回转身加速处理
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[转身间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwTurnTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);// 封包显示
                            }
                            _gameSpeed.dwTurnTick = HUtil32.GetTickCount();// 保存转身时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount();// 在线时间
                        }
                        break;
                    case Grobal2.CM_DROPITEM:// 扔东西
                        break;
                    case Grobal2.CM_PICKUP: // 捡东西   （与转身控制相连）
                        if (GateShare.boStartTurnCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwPickupTick) < GateShare.dwSpinEditPickupTime)
                            {
                                if (SessionInfo.boSendAvailable)
                                {
                                    SessionInfo.dwClientCheckTimeOut = 100;// 延迟间隔
                                }
                                else
                                {
                                    SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditPickupTime;// 延迟间隔
                                }
                                result = GateShare.dwComboBoxTurnCheck + 10; // 返回转身加速处理
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[捡起间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwPickupTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3); // 封包显示
                            }
                            _gameSpeed.dwPickupTick = HUtil32.GetTickCount(); // 保存捡起时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount(); // 在线时间
                        }
                        break;
                    case Grobal2.CM_BUTCH:// 挖肉    （只有停顿和延迟处理）
                        if (GateShare.boStartButchCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwButchTick) < GateShare.dwSpinEditButchTime)
                            {
                                if (SessionInfo.boSendAvailable)
                                {
                                    SessionInfo.dwClientCheckTimeOut = 200;// 延迟间隔
                                }
                                else
                                {
                                    SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditButchTime;// 延迟间隔
                                }
                                result = GateShare.dwComboBoxButchCheck + 10; // 返回挖肉加速处理
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[挖肉间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwButchTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);// 封包显示
                            }
                            _gameSpeed.dwButchTick = HUtil32.GetTickCount();// 保存挖肉时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount();// 在线时间
                        }
                        _gameSpeed.dwUserDoTick = 1007;
                        break;
                    case Grobal2.CM_SITDOWN:// 记录本次操作
                        _gameSpeed.dwUserDoTick = 3012;// 挖(蹲下)
                        _gameSpeed.dwGameTick = HUtil32.GetTickCount();// 记录本次操作
                        break;
                    case Grobal2.CM_EAT:
                        // 在线时间
                        // 吃药     （只有停顿和延迟处理）
                        // ------------------------------------
                        // 原理：所有可双击物品，全部视为吃药动作，
                        // 判断两次间隔超速后，可采取抛出数据包或延迟数据包执行。
                        // 使用抛出数据包会将导致，回程卷等物品也无法使用，和出现卡药现象。
                        // 使用延迟处理，未做测试，封顶药挂效果。 新添加的方案~
                        // ------------------------------------
                        if ((HUtil32.GetTickCount() - _gameSpeed.dwContinueTick) < 3000)// 停顿操作后3秒内数据全部抛出
                        {
                            result = 4;// 返回抛出数据
                            return result;
                        }
                        // ------------------------------------
                        if (GateShare.boStartEatCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwEatTick) < GateShare.dwSpinEditEatTime)
                            {
                                if (SessionInfo.boSendAvailable)// 延迟间隔
                                {
                                    SessionInfo.dwClientCheckTimeOut = 100;
                                }
                                else
                                {
                                    SessionInfo.dwClientCheckTimeOut += GateShare.dwSpinEditEatTime;// 延迟间隔
                                }
                                result = GateShare.dwComboBoxEatCheck + 10; // 返回吃药加速处理
                            }
                            if (SessionInfo.sUserName == GateShare.boMsgUserName)
                            {
                                GateShare.AddMainLogMsg("[吃药间隔]：" + (HUtil32.GetTickCount() - _gameSpeed.dwEatTick) + " 毫秒(" + (_gameSpeed.nErrorCount) + "/50)", 3);// 封包显示
                            }
                            _gameSpeed.dwEatTick = HUtil32.GetTickCount();// 保存吃药时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount();// 在线时间
                        }
                        break;
                    case Grobal2.CM_CHECKTIME: // 变速齿轮
                        // ------------------------------------
                        // 原理：SKY客户端登陆游戏后，每1分钟向游戏网关发送一次CM_15999进行验证
                        // 在这里只需要判断两次间隔小于50秒，作为客户端已被加速处理。
                        // 为了减少误封，判断加速连续2次以上，执行掉线处理。
                        // ------------------------------------
                        if (GateShare.boSupSpeederCheck)
                        {
                            if ((HUtil32.GetTickCount() - _gameSpeed.dwSupSpeederTick) < 50000)
                            {
                                if (_gameSpeed.dwSupSpeederCount > 1)
                                {
                                    _gameSpeed.dwSupSpeederCount = 0; // 清零
                                    result = 2; // 返回掉线处理
                                    return result;
                                }
                                else
                                {
                                    _gameSpeed.dwSupSpeederCount++; // 变速齿轮累计
                                }
                            }
                            _gameSpeed.dwSupSpeederTick = HUtil32.GetTickCount(); // 保存变速齿轮时间
                            _gameSpeed.dwGameTick = HUtil32.GetTickCount(); // 在线时间
                        }
                        result = 4; // 抛出处理，解决SKY登陆器 1分钟一卡问题。
                        return result;
                    case Grobal2.CM_SAY:// 说话
                        sDataText = "";
                        if ((HUtil32.GetTickCount() - SessionInfo.dwSayMsgTick) < GateShare.dwSayMsgTime) // 控制发言间隔时间
                        {
                            SendWarnMsg(SessionInfo, GateShare.gWarningMsg, GateShare.btRedMsgFColor, GateShare.btRedMsgBColor);// 提示文字警告
                            result = 4;
                            return result;
                        }
                        sDataText = EDcode.DeCodeString(sMsg);// 解密
                        if (sDataText != "")
                        {
                            if (sDataText[0] == '/')
                            {
                                sDataText = HUtil32.GetValidStr3(sDataText, ref sHumName, new string[] { " " });// 限制最长可发字符长度
                                if (sDataText.Length > GateShare.nSayMsgMaxLen)
                                {
                                    sDataText = sDataText.Substring(1 - 1, GateShare.nSayMsgMaxLen);
                                }
                                FilterSayMsg(ref sDataText);// 过滤文字
                                switch (sDataText)
                                {
                                    case "掉线处理":
                                        result = 2;
                                        return result;
                                    case "丢包处理":
                                        result = 4;
                                        return result;
                                    default:
                                        sDataText = sHumName + " " + sDataText;
                                        break;
                                }
                            }
                            else
                            {
                                if (sDataText[0] == '@') //对游戏命令放行
                                {
                                    result = -1;
                                    return result;
                                }
                                if (sDataText[0] != '@')
                                {
                                    // 限制最长可发字符长度
                                    if (sDataText.Length > GateShare.nSayMsgMaxLen)
                                    {
                                        sDataText = sDataText.Substring(1 - 1, GateShare.nSayMsgMaxLen);
                                    }
                                    FilterSayMsg(ref sDataText);// 过滤文字
                                    switch (sDataText)
                                    {
                                        case "掉线处理":
                                            result = 2;
                                            return result;
                                        case "丢包处理":
                                            result = 4;
                                            return result;
                                    }
                                }
                                else
                                {
                                    if (sDataText.IndexOf(GateShare.sModeFilter, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        if (GateShare.boFeiDnItemsCheck)// 封掉飞装备
                                        {
                                            if ((HUtil32.GetTickCount() - _gameSpeed.dwFeiDnItemsTick) < GateShare.dwSayMoveTime)
                                            {
                                                SendWarnMsg(SessionInfo, GateShare.fWarningMsg, GateShare.btYYMsgFColor, GateShare.btYYMsgBColor); // 提示文字警告
                                                result = 4; // 返回抛出数据处理
                                                return result;
                                            }
                                        }
                                        _gameSpeed.dwFeiDnItemsTick = HUtil32.GetTickCount(); // 保存传送时间
                                    }
                                    else
                                    {
                                        if (sDataText.IndexOf(GateShare.sMsgFilter, StringComparison.OrdinalIgnoreCase) != 0)// 控制发言间隔时间
                                        {
                                            if ((HUtil32.GetTickCount() - SessionInfo.dwSayMsgTick) < GateShare.TrinidadMsgTime)
                                            {
                                                SendWarnMsg(SessionInfo, GateShare.gWarningMsg, GateShare.btRedMsgFColor, GateShare.btRedMsgBColor); // 提示文字警告
                                                result = 4;
                                                return result;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        sMsg = EDcode.EncodeString(sDataText);
                        _gameSpeed.dwGameTick = HUtil32.GetTickCount(); // 在线时间
                        SessionInfo.dwSayMsgTick = HUtil32.GetTickCount();// 保存发言时间
                        break;
                }
            }
            catch
            {
                GateShare.AddMainLogMsg("[异常] TFrmMain.CheckDefMsg", 1);
            }
            return result;
        }

        /// <summary>
        /// 发送警告文字
        /// </summary>
        private void SendWarnMsg(TSessionInfo SessionInfo, string sMsg, byte FColor, byte BColor)
        {
            if ((SessionInfo == null))
            {
                return;
            }
            if ((SessionInfo.Socket == null))
            {
                return;
            }
            if (SessionInfo.Socket.Connected)
            {
                var DefMsg = Grobal2.MakeDefaultMsg(Grobal2.SM_WHISPER, (int)SessionInfo.Socket.Handle, HUtil32.MakeWord(FColor, BColor), 0, 1);
                var sSendText = "#" + EDcode.EncodeMessage(DefMsg) + EDcode.EncodeString(sMsg) + "!";
                SessionInfo.Socket.SendText(sSendText);
            }
        }

        /// <summary>
        /// 文字消息处理(过滤，已经发言间隔)
        /// </summary>
        /// <param name="sMsg"></param>
        private void FilterSayMsg(ref string sMsg)
        {
            int nLen;
            string sReplaceText;
            string sFilterText;
            try
            {
                HUtil32.EnterCriticalSection(GateShare.CS_FilterMsg);
                for (var i = 0; i < GateShare.AbuseList.Count; i++)
                {
                    sFilterText = GateShare.AbuseList[i];
                    sReplaceText = "";
                    if (sMsg.IndexOf(sFilterText, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        for (nLen = 0; nLen <= sFilterText.Length; nLen++)
                        {
                            sReplaceText = sReplaceText + GateShare.sReplaceWord;
                        }
                        sMsg = sMsg.Replace(sFilterText, sReplaceText);
                    }
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(GateShare.CS_FilterMsg);
            }
        }

        private void Send(ushort nIdent, ushort wSocketIndex, int nSocket, ushort nUserListIndex, int nLen, byte[] dataBuff)
        {
            GateShare.ForwardMsgList.Writer.TryWrite(new ForwardMessage()
            {
                nIdent = nIdent,
                wSocketIndex = wSocketIndex,
                nSocket = nSocket,
                nUserListIndex = nUserListIndex,
                nLen = nLen,
                Data = dataBuff
            });
        }
    }


    public enum TCheckStep
    {
        csCheckLogin,
        csSendCheck,
        csSendSmu,
        csSendFinsh,
        csCheckTick
    }

    public class GameSpeed
    {
        /// <summary>
        /// 加速的累计值
        /// </summary>
        public int nErrorCount;
        /// <summary>
        /// 装备加速
        /// </summary>
        public int m_nHitSpeed;
        /// <summary>
        /// 发言时间
        /// </summary>
        public long dwSayMsgTick;
        /// <summary>
        /// 移动时间
        /// </summary>
        public long dwMoveTick;
        /// <summary>
        /// 攻击时间
        /// </summary>
        public long dwAttackTick;
        /// <summary>
        /// 魔法时间
        /// </summary>
        public long dwSpellTick;
        /// <summary>
        /// 走路时间
        /// </summary>
        public long dwWalkTick;
        /// <summary>
        /// 跑步时间
        /// </summary>
        public long dwRunTick;
        /// <summary>
        /// 转身时间
        /// </summary>
        public long dwTurnTick;
        /// <summary>
        /// 挖肉时间
        /// </summary>
        public long dwButchTick;
        /// <summary>
        /// 蹲下时间
        /// </summary>
        public long dwSitDownTick;
        /// <summary>
        /// 吃药时间
        /// </summary>
        public long dwEatTick;
        /// <summary>
        /// 捡起时间
        /// </summary>
        public long dwPickupTick;
        /// <summary>
        /// 移动时间
        /// </summary>
        public long dwRunWalkTick;
        /// <summary>
        /// 传送时间
        /// </summary>
        public long dwFeiDnItemsTick;
        /// <summary>
        /// 变速齿轮时间
        /// </summary>
        public long dwSupSpeederTick;
        /// <summary>
        /// 变速齿轮累计
        /// </summary>
        public int dwSupSpeederCount;
        /// <summary>
        /// 超级加速时间
        /// </summary>
        public long dwSuperNeverTick;
        /// <summary>
        /// 超级加速累计
        /// </summary>
        public int dwSuperNeverCount;
        /// <summary>
        /// 记录上一次操作
        /// </summary>
        public int dwUserDoTick;
        /// <summary>
        /// 保存停顿操作时间
        /// </summary>
        public long dwContinueTick;
        /// <summary>
        /// 带有攻击并发累计
        /// </summary>
        public int dwConHitMaxCount;
        /// <summary>
        /// 带有魔法并发累计
        /// </summary>
        public int dwConSpellMaxCount;
        /// <summary>
        /// 记录上一次移动方向
        /// </summary>
        public int dwCombinationTick;
        /// <summary>
        /// 智能攻击累计
        /// </summary>
        public int dwCombinationCount;
        public long dwGameTick;

        public GameSpeed()
        {

        }
    }
}