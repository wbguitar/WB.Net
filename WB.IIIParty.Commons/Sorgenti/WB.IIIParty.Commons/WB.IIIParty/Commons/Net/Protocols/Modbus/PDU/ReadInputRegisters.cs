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
    public class ReadInputRegisters : ModbusPDU
    {
        private const byte functionCode = 4;

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

            byte[] wordCount = BitConverter.GetBytes((Int16)point.GetMbSize());
            tempSwap = wordCount[1];
            wordCount[1] = wordCount[0];
            wordCount[0] = tempSwap;

            int index = 0;
            int size = 1 + refNum.Length + wordCount.Length;
            result = new byte[size];

            result[0] = functionCode;
            index++;
            Array.Copy(refNum, 0, result, index, refNum.Length);
            index += refNum.Length;
            Array.Copy(wordCount, 0, result, index, wordCount.Length);
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

                point.SetMbSize((int)(responseData[index] / 2));
                index++;

                byte[] rv = new byte[2];
                int j = index;
                for (int i = 0; i < point.GetMbSize(); i++)
                {
                    rv[1] = (byte)responseData[j];
                    rv[0] = (byte)responseData[++j];
                    point.GetMbPointValue().Add((Int16)BitConverter.ToInt16(rv, 0));
                    j++;
                }
            }
            else
            {
                ((ModbusPoint)point).SetMbExceptionCode(1);
            }
        }
    }
}
