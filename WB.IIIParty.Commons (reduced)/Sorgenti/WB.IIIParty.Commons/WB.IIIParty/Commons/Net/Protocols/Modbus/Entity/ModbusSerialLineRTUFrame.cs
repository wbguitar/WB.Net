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
using WB.IIIParty.Commons.Net.Protocols.Modbus.Utility;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class ModbusSerialLineRTUFrame : IModbusSerialLineFrame
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private byte[] destination = new byte[1];
        /// <summary>
        /// 
        /// </summary>
        private byte[] data;
        /// <summary>
        /// 
        /// </summary>
        private byte[] cRC = new byte[2];

        #endregion

        #endregion

        #region Public Members

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public byte[] Destination
        {
            get { return destination; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] CRC
        {
            get { return cRC; }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="dataSend"></param>
        public byte[] CreateFrame(Int16 dest, byte[] dataSend)
        {
            byte[] tmp = new byte[2];
            tmp = BitConverter.GetBytes(dest);
            destination[0] = tmp[0];
            int size = dataSend.Length + 3;
            data = new byte[size];
            int index = 0;
            Array.Copy(destination, 0, data, 0, 1);
            index++;
            Array.Copy(dataSend, 0, data, index, dataSend.Length);
            index += dataSend.Length;

            byte[] dataNoCRC = new byte[size - 2];
            Array.Copy(data, 0, dataNoCRC, 0, dataNoCRC.Length);
            cRC = SerialLineUtil.CreateCRC(dataNoCRC);
            tmp[0] = cRC[1];
            tmp[1] = cRC[0];
            Array.Copy(tmp, 0, data, index, 2);
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReceive"></param>
        public byte[] ParseFrame(byte[] dataReceive)
        {
            destination[0] = dataReceive[0];

            int size = dataReceive.Length - 3;
            data = new byte[size];

            Array.Copy(dataReceive, 1, data, 0, size);

            cRC[0] = dataReceive[size + 2];
            cRC[1] = dataReceive[size + 1];

            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReceive"></param>
        /// <returns></returns>
        public bool CheckFrame(byte[] dataReceive)
        {
            byte[] dataNoCRC = new byte[dataReceive.Length - 2];
            Array.Copy(dataReceive, 0, dataNoCRC, 0, dataNoCRC.Length);
            byte[] cRCData = SerialLineUtil.CreateCRC(dataNoCRC);
            byte[] tmp = new byte[2];
            tmp[0] = cRCData[1];
            tmp[1] = cRCData[0];

            if ((tmp[0] == dataReceive[dataReceive.Length - 2]) && (tmp[1] == dataReceive[dataReceive.Length - 1]))
                return true;
            else
                return false;
        }

        #endregion

        #endregion
    }
}
