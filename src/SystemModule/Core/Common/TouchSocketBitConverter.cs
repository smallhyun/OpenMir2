using System;
using System.Runtime.CompilerServices;
using SystemModule.Core.Common.Enum;

namespace SystemModule.Core.Common
{
    /// <summary>
    /// 将基数据类型转换为指定端的一个字节数组，
    /// 或将一个字节数组转换为指定端基数据类型。
    /// </summary>
    public class TouchSocketBitConverter
    {
        /// <summary>
        /// 以大端
        /// </summary>
        public static TouchSocketBitConverter BigEndian;

        /// <summary>
        /// 以小端
        /// </summary>
        public static TouchSocketBitConverter LittleEndian;

        static TouchSocketBitConverter()
        {
            BigEndian = new TouchSocketBitConverter(EndianType.Big);
            LittleEndian = new TouchSocketBitConverter(EndianType.Little);
            DefaultEndianType = EndianType.Little;
        }

        private static TouchSocketBitConverter m_default;

        /// <summary>
        /// 以默认小端，可通过<see cref="TouchSocketBitConverter.DefaultEndianType"/>重新指定默认端。
        /// </summary>
        public static TouchSocketBitConverter Default => m_default;

        private static EndianType m_defaultEndianType;

        /// <summary>
        /// 默认大小端切换。
        /// </summary>
        public static EndianType DefaultEndianType
        {
            get => m_defaultEndianType;
            set
            {
                m_defaultEndianType = value;
                switch (value)
                {
                    case EndianType.Little:
                        m_default = LittleEndian;
                        break;

                    case EndianType.Big:
                        m_default = BigEndian;
                        break;

                    default:
                        break;
                }
            }
        }

        private readonly EndianType endianType;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="endianType"></param>
        public TouchSocketBitConverter(EndianType endianType)
        {
            this.endianType = endianType;
        }

        /// <summary>
        /// 指定大小端。
        /// </summary>
        public EndianType EndianType => endianType;

        /// <summary>
        /// 判断当前系统是否为设置的大小端
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSameOfSet()
        {
            return !(BitConverter.IsLittleEndian ^ (endianType == EndianType.Little));
            //return true;
        }

        #region ushort

        /// <summary>
        /// 转换为指定端2字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// 转换为指定端模式的2字节转换为UInt16数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ushort ToUInt16(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToUInt16(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[2];
                Array.Copy(buffer, offset, bytes, 0, 2);
                Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes, 0);
            }
        }

        #endregion ushort

        #region ulong

        /// <summary>
        /// 转换为指定端8字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的Ulong数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ulong ToUInt64(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToUInt64(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[8];
                Array.Copy(buffer, offset, bytes, 0, 8);
                Array.Reverse(bytes);
                return BitConverter.ToUInt64(bytes, 0);
            }

        }

        #endregion ulong

        #region bool

        /// <summary>
        /// 转换为指定端1字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///  转换为指定端模式的bool数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool ToBoolean(byte[] buffer, int offset)
        {
            return BitConverter.ToBoolean(buffer, offset);
        }

        #endregion bool

        #region char

        /// <summary>
        /// 转换为指定端2字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(char value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的Char数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public char ToChar(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToChar(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[2];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToChar(bytes, 0);
            }
        }

        #endregion char

        #region short

        /// <summary>
        /// 转换为指定端2字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的Short数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public short ToInt16(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToInt16(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[2];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToInt16(bytes, 0);
            }

        }

        #endregion short

        #region int

        /// <summary>
        /// 转换为指定端4字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的int数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int ToInt32(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToInt32(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[4];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        #endregion int

        #region long

        /// <summary>
        /// 转换为指定端8字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的long数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public long ToInt64(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToInt64(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[8];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToInt64(bytes, 0);
            }

        }

        #endregion long

        #region uint

        /// <summary>
        /// 转换为指定端4字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的Uint数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public uint ToUInt32(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToUInt32(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[4];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
        }

        #endregion uint

        #region float

        /// <summary>
        /// 转换为指定端4字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的float数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public float ToSingle(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToSingle(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[4];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }
        }

        #endregion float

        #region long

        /// <summary>
        /// 转换为指定端8字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        ///  转换为指定端模式的double数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public double ToDouble(byte[] buffer, int offset)
        {
            if (IsSameOfSet())
            {
                return BitConverter.ToDouble(buffer, offset);
            }
            else
            {
                byte[] bytes = new byte[8];
                Array.Copy(buffer, offset, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToDouble(bytes, 0);
            }
        }

        #endregion long
    }
}