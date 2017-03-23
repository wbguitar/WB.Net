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
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.PDU
{
    /// <summary>
    /// 
    /// </summary>
    public class WriteMultipleRegisters : ModbusPDU
    {
        private byte functionCode = 16;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override byte[] MbCreateReqPDU(IModbusPoint point)
        {
            byte[] result;
            byte tempSwap = 0;
            byte[] tmp = new byte[2];
        
            byte[] refNum = BitConverter.GetBytes((Int16)point.GetMbAddress());
            tempSwap = refNum[1];
            refNum[1] = refNum[0];
            refNum[0] = tempSwap;

            byte[] wordCount = BitConverter.GetBytes((Int16)(point.GetMbSize()));
            tempSwap = wordCount[1];
            wordCount[1] = wordCount[0];
            wordCount[0] = tempSwap;

            tmp = BitConverter.GetBytes((Int16)((point.GetMbSize()) * 2));
            byte byteCount = tmp[0];

            byte[] registerValue = new byte[(int)byteCount];

            byte[] ival = new byte[2];
            int j = 0;
            for (int i = 0; i < point.GetMbSize(); i++)
            {
                if ((((ModbusPoint)point).GetMbWriteValue()).ContainsKey(i))
                {
                    ival = BitConverter.GetBytes((Int16)(point.GetMbWriteValue()[i]));
                }
                else
                {
                    ival = BitConverter.GetBytes((Int16)(point.GetMbPointValue()[i]));
                }
                
                registerValue[j] = ival[1];
                j++;
                registerValue[j] = ival[0];
                j++;
            }

            int index = 0;
            int size = 1 + refNum.Length + wordCount.Length + 1 + registerValue.Length;
            result = new byte[size];

            result[0] = functionCode;
            index++;
            Array.Copy(refNum, 0, result, index, refNum.Length);
            index += refNum.Length;
            Array.Copy(wordCount, 0, result, index, wordCount.Length);
            index += wordCount.Length;
            result[index] = byteCount;
            index++;
            Array.Copy(registerValue, 0, result, index, registerValue.Length);
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
            byte fc = responseData[0];

            if (fc == functionCode)
            {
                byte[] tmp = new byte[2];

                tmp[0] = (byte)responseData[2];
                tmp[1] = (byte)responseData[1];
                Int16 rn = BitConverter.ToInt16(tmp, 0);

                point.SetMbAddress((int)rn);

                tmp[0] = (byte)responseData[4];
                tmp[1] = (byte)responseData[3];
                Int16 wc = BitConverter.ToInt16(tmp, 0);

                ((ModbusPoint)point).SetMbSize((int)wc);
            }
            else
            {
                ((ModbusPoint)point).SetMbExceptionCode(1);
            }
        }
    }
}
