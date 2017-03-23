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
    public class WriteMultipleCoils : ModbusPDU
    {
        private const byte functionCode = 15;
       
        /// <summary>
        /// Creazione del PDU di richiesta Modbus
        /// </summary>
        /// <param name="point">Punto Modbus</param>
        /// <returns>Serializzazione del PDU di richiesta Modbus</returns>
        public override byte[] MbCreateReqPDU(IModbusPoint point)
        {

            byte[] result;
            byte tempSwap = 0;
            byte[] tmp = new byte[2];

            byte[] refNum = BitConverter.GetBytes((Int16)point.GetMbAddress());
            tempSwap = refNum[1];
            refNum[1] = refNum[0];
            refNum[0] = tempSwap;

            byte[] bitCount = BitConverter.GetBytes((Int16)point.GetMbSize());
            tempSwap = bitCount[1];
            bitCount[1] = bitCount[0];
            bitCount[0] = tempSwap;


            tmp = BitConverter.GetBytes((Int16)((point.GetMbSize() + 7) / 8));
            byte byteCount = tmp[0];

            Int16 val = 0;
            byte[] outputValue = new byte[(int)byteCount];

            for (int i = 0; i < point.GetMbSize() / 8; i++)
            {
                for (int j = i * 8; j < (i * 8) + 8; j++)
                {
                    int boolValue = 0;
                    if (((ModbusPoint)point).GetMbWriteValue().ContainsKey(j))
                    {
                        if ((bool)((ModbusPoint)point).GetMbWriteValue()[j])
                        {
                            boolValue = 1;
                        }
                        else
                        {
                            boolValue = 0;
                        }
                    }
                    else
                    {
                        if ((bool)point.GetMbPointValue()[j])
                        {
                            boolValue = 1;
                        }
                        else
                        {
                            boolValue = 0;
                        }
                    }
                    int esp = (j % 8);
                    val = (Int16)(val + (Int16)(boolValue * (System.Math.Pow(2, esp))));
                }

                tmp = BitConverter.GetBytes((Int16)val);
                outputValue[i] = tmp[0];
                val = 0;
            }

            int index = 0;
            int size = 1 + refNum.Length + bitCount.Length + 1 + outputValue.Length;
            result = new byte[size];

            result[0] = functionCode;
            index++;
            Array.Copy(refNum, 0, result, index, refNum.Length);
            index += refNum.Length;
            Array.Copy(bitCount, 0, result, index, bitCount.Length);
            index += bitCount.Length;
            result[index] = byteCount;
            index++;
            Array.Copy(outputValue, 0, result, index, outputValue.Length);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="point">Punto Modbus</param>
        public override void MbParseReqPDU(byte[] requestData, ref IModbusPoint point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Creazione del PDU di richiesta Modbus
        /// </summary>
        /// <param name="point">Punto Modbus</param>
        /// <returns>Serializzazione del PDU di richiesta Modbus</returns>
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
                Int16 bc = BitConverter.ToInt16(tmp, 0);

                ((ModbusPoint)point).SetMbSize((int)bc);
            }
            else
            {
                ((ModbusPoint)point).SetMbExceptionCode(1);
            }
        }
    }
}
