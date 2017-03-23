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
using System.Collections;
using System.Text;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.PDU
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadInputDiscretes : ModbusPDU
    {
        private byte functionCode = 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override byte[] MbCreateReqPDU(IModbusPoint point)
        {
            byte[] result;
            byte tempSwap = 0;

            byte[] refNum = BitConverter.GetBytes((Int16)point.GetMbAddress());
            tempSwap = refNum[1];
            refNum[1] = refNum[0];
            refNum[0] = tempSwap;

            byte[] bitCount = BitConverter.GetBytes((Int16)point.GetMbSize());
            tempSwap = bitCount[1];
            bitCount[1] = bitCount[0];
            bitCount[0] = tempSwap;

            int index = 0;
            int size = 1 + refNum.Length + bitCount.Length;
            result = new byte[size];

            result[0] = functionCode;
            index++;
            Array.Copy(refNum, 0, result, index, refNum.Length);
            index += refNum.Length;
            Array.Copy(bitCount, 0, result, index, bitCount.Length);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="point"></param>
        public override void MbParseReqPDU(byte[] requestData, ref IModbusPoint point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override byte[] MbCreateRspPDU(IModbusPoint point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="point"></param>
        public override void MbParseRspPDU(byte[] responseData, ref IModbusPoint point)
        {
            int index = 0;

            byte fc = responseData[index];
            index++;
            if (fc == functionCode)
            {
                //  Estraggo il numero di byte di risposta:
                byte byteCount = responseData[index];
                index++;
                //  Creo un array di byte da convertire in BitArray:
                byte[] data = new byte[byteCount];
                //  Copio i dati ricevuti:
                Array.Copy(responseData, index, data, 0, byteCount);
                //  Estraggo il BitArray
                System.Collections.BitArray bitArray = new System.Collections.BitArray(data);

                for (int i = 0; i < point.GetMbSize(); i++)
                {
                    point.GetMbPointValue()[i] = bitArray.Get(i);
                }
            }
            else
            {
                ((ModbusPoint)point).SetMbExceptionCode(1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myList"></param>
        /// <param name="result"></param>
        private void Convert(IEnumerable myList, ref Int16[] result)
        {
            int i = 0;
            foreach (Object obj in myList)
            {
                if (obj.Equals(true))
                    result[i] = 1;
                else
                    result[i] = 0;
                i++;
            }
        }
    }
}
