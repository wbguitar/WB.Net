// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Bernini Francesco
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Utility
{
    /// <summary>
    /// Modalità di trasmissione seriale Modbus
    /// </summary>
    public enum SerialLineTransmissionMode
    {
        /// <summary>
        /// RTU
        /// </summary>
        RTU = 0,
        /// <summary>
        /// ASCII
        /// </summary>
        ASCII = 1
    }
    /// <summary>
    ///  Baud Rate seriale Modbus
    /// </summary>
    public enum SerialLineBaudRate
    {
        /// <summary>
        /// Baud Rate 9600
        /// </summary>
        BR_9600  = 9600,
        /// <summary>
        /// Baud Rate 14400
        /// </summary>
        BR_14400 = 14400,
        /// <summary>
        /// Baud Rate 19200
        /// </summary>
        BR_19200 = 19200,
        /// <summary>
        /// Baud Rate 38400
        /// </summary>
        BR_38400 = 38400,
        /// <summary>
        /// Baud Rate 56000
        /// </summary>
        BR_56000 = 56000
    }
    /// <summary>
    /// Data Bits seriale Modbus
    /// </summary>
    public enum SerialLineDataBits
    {
        /// <summary>
        /// Bata Bit 7
        /// </summary>
        DB_7 = 7,
        /// <summary>
        /// Data Bit 8
        /// </summary>
        DB_8 = 8
    }
    /// <summary>
    /// Stop Bits seriale Modbus
    /// </summary>
    public enum SerialLineStopBits
    {
        /// <summary>
        /// Stop Bit 0
        /// </summary>
        SB_1    = 0,
        /// <summary>
        /// Stop Bit 1
        /// </summary>
        SB_1_5  = 1,
        /// <summary>
        /// Stop Bit 2
        /// </summary>
        SB_2    = 2
    }
    /// <summary>
    /// Parity seriale Modbus
    /// </summary>
    public enum SerialLineParity
    {
        /// <summary>
        /// Nessuna Parità
        /// </summary>
        NONE    = 0,
        /// <summary>
        /// Parità Pari
        /// </summary>
        ODD     = 1,
        /// <summary>
        /// Parità Dispari
        /// </summary>
        EVEN    = 2,
        /// <summary>
        /// Parità Mark
        /// </summary>
        MARK    = 3,
        /// <summary>
        /// Parità Space
        /// </summary>
        SPACE   = 4
    }
    /// <summary>
    /// Classe contenente metodi statici di utilità
    /// </summary>
    public class SerialLineUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CreateCRC(byte[] data)
        {
            byte[] crcReg = new byte[2];
            crcReg[0] = 255;
            crcReg[1] = 255;
            byte[] poly = new byte[2];
            poly[0] = 160;
            poly[1] = 1;
            int lsb = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crcReg[1] = (byte)(crcReg[1] ^ data[i]);
                crcReg[0] = (byte)(crcReg[0] ^ 0);
                for (int j = 0; j < 8; j++)
                {
                    lsb = ShiftRightOne(crcReg);
                    if (lsb == 1)
                    {
                        crcReg[0] = (byte)(crcReg[0] ^ poly[0]);
                        crcReg[1] = (byte)(crcReg[1] ^ poly[1]);
                    }
                }

            }
            return crcReg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int ShiftRightOne(byte[] data)
        {
            int res = 0;
            if ((data[0] % 2) == 0)
            {
                if ((data[1] % 2) == 0)
                    res = 0;
                else res = 1;
                data[1] = (byte)(data[1] >> 1);
                data[0] = (byte)(data[0] >> 1);
                return res;
            }
            else
            {
                if ((data[1] % 2) == 0)
                    res = 0;
                else res = 1;
                data[1] = (byte)(data[1] >> 1);
                data[1] = (byte)(data[1] + 128);
                data[0] = (byte)(data[0] >> 1);
                return res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte CreateLRC(byte[] data)
        {
            byte result = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                result = (byte)(result + data[i]);
            }
            result = (byte)(255 - result);
            result++;
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertToASCII(byte[] data)
        {
            byte[] result = new byte[data.Length * 2];
            byte[] ASCIIValue = new byte[2];
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();

            for (int i = 0; i < data.Length; i++)
            {
                ASCIIValue = ascii.GetBytes(string.Format("{0:X1}{1:X1}", ((data[i] >> 4) & 0xF), (data[i] & 0xF)));
                result[(i * 2)] = ASCIIValue[0];
                result[(i * 2) + 1] = ASCIIValue[1];
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertFromASCII(byte[] data)
        {
            string ASCIIValue;

            byte[] result = new byte[data.Length / 2];

            for (int i = 0; i < (data.Length) / 2; i++)
            {
                ASCIIValue = string.Format("0x{0}{1}", (char)data[i * 2], (char)data[(i * 2) + 1]);
                result[i] = Convert.ToByte(ASCIIValue, 16);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReceived"></param>
        /// <param name="transmissionMode"></param>
        /// <returns></returns>
        public static byte[] CheckResponseReceivedOnSerialLine(byte[] dataReceived, SerialLineTransmissionMode transmissionMode)
        {
            byte[] error = new byte[3];
            switch (transmissionMode)
            {
                case SerialLineTransmissionMode.RTU:
                    {
                        error[0] = dataReceived[0];
                        int fcExceptionCode = 128 + (int)dataReceived[1];
                        error[1] = (byte)fcExceptionCode;
                        error[2] = 3;
                        byte[] crc = CreateCRC(error);
                        byte[] exception = new byte[5];
                        Array.Copy(error, 0, exception, 0, error.Length);
                        exception[3] = crc[1];
                        exception[4] = crc[0];
                        //	Controllo dei dati ricevuti:
                        int fc = dataReceived[1];
                        if ((fc >= 1) && (fc <= 4))
                        {
                            if (dataReceived.Length > 5)
                            {
                                int nrByte = dataReceived[2];
                                if (dataReceived.Length == nrByte + 5)
                                    return dataReceived;
                                else
                                    return exception;
                            }
                            else
                                return exception;
                        }
                        else
                            return dataReceived;
                    }
                case SerialLineTransmissionMode.ASCII:
                    {
                        byte[] tmp = new byte[dataReceived.Length - 4];
                        Array.Copy(dataReceived, 1, tmp, 0, dataReceived.Length - 4);
                        byte[] frame = ConvertFromASCII(tmp);
                        error[0] = frame[0];
                        int fcExceptionCode = 128 + (int)frame[1];
                        error[1] = (byte)fcExceptionCode;
                        error[2] = 3;
                        byte lrc = CreateLRC(error);
                        byte[] exception = new byte[4];
                        Array.Copy(error, 0, exception, 0, error.Length);
                        exception[3] = lrc;
                        //	Controllo dei dati ricevuti:
                        int fc = frame[1];
                        if ((fc >= 1) && (fc <= 4))
                        {
                            if (frame.Length > 5)
                            {
                                int nrByte = frame[2];
                                if (frame.Length == nrByte + 3)
                                    return dataReceived;
                                else
                                    return ConvertToASCII(exception);
                            }
                            else
                                return ConvertToASCII(exception);
                        }
                        else
                            return dataReceived;
                    }
                default:
                    return error;
            }
        }
    }
}
