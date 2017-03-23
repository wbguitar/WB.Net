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
    /// Frame ASCII Modbus seriale
    /// </summary>
    public class ModbusSerialLineASCIIFrame : IModbusSerialLineFrame
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private byte[] start = new byte[1];
        /// <summary>
        /// 
        /// </summary>
        private byte[] destination = new byte[2];
        /// <summary>
        /// 
        /// </summary>
        private byte[] data;
        /// <summary>
        /// 
        /// </summary>
        private byte[] lRC = new byte[2];
        /// <summary>
        /// 
        /// </summary>
        private byte[] end = new byte[2];

        #endregion

        #endregion

        #region Public Members

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public byte[] CStart
        {
            get { return this.start; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Destination
        {
            get { return this.destination; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data
        {
            get { return this.data; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] LRC
        {
            get { return this.lRC; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] CEnd
        {
            get { return this.end; }
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
            //  Array di byte ausiliari rispettivamente di 1 e 2 elementi:
            byte[] tmp1 = new byte[1];
            byte[] tmp2 = new byte[2];

            //  Calcolo dell'LRC su: dest + dataSend (NON ANCORA CONVERTITI IN ASCII!!!!!!).
            int size = dataSend.Length + 1;
            byte[] data4LRC = new byte[size];

            tmp2 = BitConverter.GetBytes(dest);
            tmp1[0] = tmp2[0];

            int index = 0;

            Array.Copy(tmp1, 0, data4LRC, index, tmp1.Length);
            index += tmp1.Length;
            Array.Copy(dataSend, 0, data4LRC, index, dataSend.Length);
            //  Calcolo:
            tmp1[0] = SerialLineUtil.CreateLRC(data4LRC);
            //  Conversione in ASCII:
            this.lRC = SerialLineUtil.ConvertToASCII(tmp1);

            //  Carattere di inizio messaggio (':') :
            char cStart = ':';
            start[0] = (byte)cStart;

            //  Carattere di fine messaggio ('CRLF') :
            end[0] = 13;
            end[1] = 10;

            //  Conversione in ASCII del messaggio:
            byte[] tmpN = SerialLineUtil.ConvertToASCII(data4LRC);

            //  Creazione del pacchetto da inviare:
            size = start.Length + tmpN.Length + lRC.Length + end.Length;
            //size = tmpN.Length + lRC.Length;

            data = new byte[size];

            index = 0;

            Array.Copy(start, 0, data, 0, start.Length);
            index += start.Length;
            Array.Copy(tmpN, 0, data, index, tmpN.Length);
            index += tmpN.Length;
            Array.Copy(lRC, 0, data, index, lRC.Length);
            index += lRC.Length;
            Array.Copy(end, 0, data, index, end.Length);
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReceive"></param>
        public byte[] ParseFrame(byte[] dataReceive)
        {
            byte[] tmp1 = new byte[1];
            byte[] tmp2 = new byte[2];

            start[0] = dataReceive[0];

            tmp2[0] = dataReceive[1];
            tmp2[1] = dataReceive[2];
            destination = SerialLineUtil.ConvertFromASCII(tmp2);

            int size = dataReceive.Length - 7;
            byte[] tmpN = new byte[size];
            Array.Copy(dataReceive, 3, tmpN, 0, size);
            data = SerialLineUtil.ConvertFromASCII(tmpN);

            Array.Copy(dataReceive, tmpN.Length + 3, tmp2, 0, tmp2.Length);
            lRC = SerialLineUtil.ConvertFromASCII(tmp2);

            Array.Copy(dataReceive, tmpN.Length + 5, end, 0, end.Length);

            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReceive"></param>
        /// <returns></returns>
        public bool CheckFrame(byte[] dataReceive)
        {
            byte[] dataNoLRCASCII = new byte[(dataReceive.Length - 5)];
            Array.Copy(dataReceive, 1, dataNoLRCASCII, 0, dataNoLRCASCII.Length);
            byte[] dataNoLRC = SerialLineUtil.ConvertFromASCII(dataNoLRCASCII);
            byte lRCData = SerialLineUtil.CreateLRC(dataNoLRC);
            byte[] tmp = new byte[2];
            tmp[0] = dataReceive[dataReceive.Length - 4];
            tmp[1] = dataReceive[dataReceive.Length - 3];
            byte[] lRCRec = SerialLineUtil.ConvertToASCII(tmp);
            if (lRCData == lRCRec[0])
                return true;
            else
                return false;
        }

        #endregion

        #endregion
    }
}
