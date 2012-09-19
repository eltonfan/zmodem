using System;

namespace ZModem_Protocol
{

    /// <summary>
    /// Class used for ZModem crc checks
    /// </summary>
    public class Crc16Ccitt
    {

        /// <summary>
        /// Initial value for Crc16Ccitt computation
        /// </summary>
        public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }


        const ushort poly = 4129;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = this.initialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public Crc16Ccitt(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }
    }

    //public class Crc16
    //{
    //    const ushort polynomial = 0xA001;
    //    //const ushort polynomial = 0x1021;
    //    ushort[] table = new ushort[256];

    //    public ushort ComputeChecksum(byte[] bytes)
    //    {
    //        ushort crc = 0;
    //        for (int i = 0; i < bytes.Length; ++i)
    //        {
    //            byte index = (byte)(crc ^ bytes[i]);
    //            crc = (ushort)((crc >> 8) ^ table[index]);
    //        }
    //        return crc;
    //    }

    //    public byte[] ComputeChecksumBytes(byte[] bytes)
    //    {
    //        ushort crc = ComputeChecksum(bytes);
    //        return BitConverter.GetBytes(crc);
    //    }

    //    public Crc16()
    //    {
    //        ushort value;
    //        ushort temp;
    //        for (ushort i = 0; i < table.Length; ++i)
    //        {
    //            value = 0;
    //            temp = i;
    //            for (byte j = 0; j < 8; ++j)
    //            {
    //                if (((value ^ temp) & 0x0001) != 0)
    //                {
    //                    value = (ushort)((value >> 1) ^ polynomial);
    //                }
    //                else
    //                {
    //                    value >>= 1;
    //                }
    //                temp >>= 1;
    //            }
    //            table[i] = value;
    //        }
    //    }
    //}

    public partial class ZModem
    {
        private static int ComputeHeaderCRC(int type, int arg0, int arg1, int arg2, int arg3)
        {
            //Calcul du crc
            Byte[] b = new Byte[5];
            b[0] = Convert.ToByte((int)type);
            b[1] = Convert.ToByte(arg0);
            b[2] = Convert.ToByte(arg1);
            b[3] = Convert.ToByte(arg2);
            b[4] = Convert.ToByte(arg3);
            int ret = calcrc(b, b.Length);

            return ret;
        }

        private static bool ComputeHexHeaderCRC(ref string HexFrame)
        {
            if (HexFrame.Length != 14)
                return false;
            if (HexFrame[0] != Convert.ToChar(ControlBytes.ZPAD)) //Header Hex
                return false;
            if (HexFrame[1] != Convert.ToChar(ControlBytes.ZPAD)) //Header Hex
                return false;
            if (HexFrame[2] != Convert.ToChar(ControlBytes.ZDLE)) //Header Hex
                return false;
            if (HexFrame[3] != Convert.ToChar(ControlBytes.ZHEX)) //Header Hex
                return false;

            int crc = ComputeHeaderCRC(Convert.ToInt32(HexFrame.Substring(4, 2), 16), Convert.ToInt32(HexFrame.Substring(6, 2), 16),
                Convert.ToInt32(HexFrame.Substring(8, 2), 16), Convert.ToInt32(HexFrame.Substring(10, 2), 16), Convert.ToInt32(HexFrame.Substring(12, 2), 16));

            HexFrame += crc.ToString("x4");
            return true;
        }

        private static int calcrc(Byte[] data, int count)
        {
            int crc = 0;
            int d;
            for (int i = 0; i < count; i++)
            {
                d = data[i];
                crc = crc ^ (d << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (crc << 1) ^ 0x1021;
                    else
                        crc = (crc << 1);
                }
            }
            return (crc & 0xFFFF);
        }
    }
}

