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
using WB.IIIParty.Commons.Net.Protocols.Modbus.PDU;
using WB.IIIParty.Commons.Logger;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Entity
{
    /// <summary>
    /// Risposta Modbus
    /// </summary>
    public class ModbusResponse
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private IModbusPoint mbPoint;
        /// <summary>
        /// 
        /// </summary>
        private byte[] readResponse;
        /// <summary>
        /// 
        /// </summary>
        private byte[] writeResponse;
        /// <summary>
        /// 
        /// </summary>
        private object[] readData;
        /// <summary>
        /// 
        /// </summary>
        private ModbusSettings  mbSettings = ModbusSettings.GetInstance();
        /// <summary>
        /// Oggetto per la creazione dei messaggi di log.
        /// </summary>
        private IMessageLog messageLog;

        #endregion

        #region Private Method

        /// <summary>
        /// 
        /// </summary>
        private void Create()
        {
            try
            {
                int functionCode = 0;
                System.Collections.ArrayList parameter = new System.Collections.ArrayList();
                switch (this.mbPoint.GetMbPointType())
                {
                    case ModbusPointType.CoilStatus:
                        {
                            switch (this.mbPoint.GetMbAccessMode())
                            {
                                case ModbusAccessMode.Read:
                                    {
                                        //  ReadCoils:
                                        functionCode = 1;
                                        for (int i = 0; i < this.readData.Length; i++)
                                        {
                                            this.mbPoint.GetMbPointValue().Add(this.readData[i]);
                                        }
                                        //  Richiesta di lettura:
                                        this.readResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.Write:
                                    {
                                        //  WriteMultipleCoils
                                        functionCode = 15;
                                        //  Richiesta di scrittura:
                                        this.writeResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.ReadWrite:
                                    {
                                        //  ReadCoils:
                                        functionCode = 1;
                                        for (int i = 0; i < this.readData.Length; i++)
                                        {
                                            this.mbPoint.GetMbPointValue().Add(this.readData[i]);
                                        }
                                        //  Richiesta di lettura:
                                        this.readResponse = this.GetByteArray(functionCode);

                                        parameter.Clear();

                                        //  WriteMultipleCoils
                                        functionCode = 15;
                                        //  Richiesta di scrittura:
                                        this.writeResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                            }
                        }
                        break;
                    case ModbusPointType.InputStatus:
                        {
                            switch (this.mbPoint.GetMbAccessMode())
                            {
                                case ModbusAccessMode.Read:
                                    {
                                        //  InputStatus:
                                        functionCode = 2;
                                        for (int i = 0; i < this.readData.Length; i++)
                                        {
                                            this.mbPoint.GetMbPointValue().Add(this.readData[i]);
                                        }
                                        //  Richiesta di lettura:
                                        this.readResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                            }
                        }
                        break;
                    case ModbusPointType.InputRegister:
                        {
                            switch (this.mbPoint.GetMbAccessMode())
                            {
                                case ModbusAccessMode.Read:
                                    {
                                        //  InputRegister:
                                        functionCode = 4;
                                        for (int i = 0; i < this.readData.Length; i++)
                                        {
                                            this.mbPoint.GetMbPointValue().Add(this.readData[i]);
                                        }
                                        //  Richiesta di lettura:
                                        this.readResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                            }
                        }
                        break;
                    case ModbusPointType.HoldingRegister:
                        {
                            switch (this.mbPoint.GetMbAccessMode())
                            {
                                case ModbusAccessMode.Read:
                                    {
                                        //  HoldingRegister:
                                        functionCode = 3;
                                        for (int i = 0; i < this.readData.Length; i++)
                                        {
                                            this.mbPoint.GetMbPointValue().Add(this.readData[i]);
                                        }
                                        //  Richiesta di lettura:
                                        this.readResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.Write:
                                    {
                                        //  WriteMultipleRegister
                                        functionCode = 16;
                                        //  Richiesta di scrittura:
                                        this.writeResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.ReadWrite:
                                    {
                                        //  HoldingRegister:
                                        functionCode = 3;
                                        for (int i = 0; i < this.readData.Length; i++)
                                        {
                                            this.mbPoint.GetMbPointValue().Add(this.readData[i]);
                                        }
                                        //  Richiesta di lettura:
                                        this.readResponse = this.GetByteArray(functionCode);

                                        parameter.Clear();

                                        //  WriteMultipleRegister
                                        functionCode = 16;
                                        //  Richiesta di scrittura:
                                        this.writeResponse = this.GetByteArray(functionCode);
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusRsponse: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void Parse(byte[] message)
        {
            try
            {
                int functionCode = (int)message[0];
                if (functionCode > 128)
                {
                    this.mbPoint.SetMbExceptionCode((int)message[1]);
                    this.Log(LogLevels.Error, "ModbusRsponse: - Exception response");
                }
                else
                {
                    System.Reflection.ConstructorInfo ci = (System.Reflection.ConstructorInfo)mbSettings.ModbusCommandType[functionCode];
                    object[] param = new object[0];
                    object obj = ci.Invoke(param);
                    ModbusPDU pdu = (ModbusPDU)obj;
                    pdu.MbParseRspPDU(message, ref this.mbPoint);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusRsponse: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        private byte[] GetByteArray(int functionCode)
        {
            System.Reflection.ConstructorInfo ci = (System.Reflection.ConstructorInfo)mbSettings.ModbusCommandType[functionCode];
            object[] param = new object[0];
            object obj = ci.Invoke(param);
            ModbusPDU pdu = (ModbusPDU)obj;
            return pdu.MbCreateReqPDU(this.mbPoint);
        }
        /// <summary>
        /// Metodo di utilizzo di messageLog.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logMessage"></param>
        private void Log(LogLevels logLevel, string logMessage)
        {
            if (this.messageLog != null)
            {
                this.messageLog.Log(logLevel, this, logMessage);
            }
        }

        #endregion

        #endregion

        #region Public Members

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="msgLog"></param>
        public ModbusResponse(ModbusPoint point, IMessageLog msgLog)
        {
            this.messageLog = msgLog;
            this.mbPoint = point;
            this.readResponse    = new byte[1] { 0 };
            this.writeResponse   = new byte[1] { 0 };
            this.Create();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="point"></param>
        /// <param name="msgLog"></param>
        public ModbusResponse(byte[] response, IModbusPoint point, IMessageLog msgLog)
        {
            this.messageLog = msgLog;
            this.mbPoint = point;
            this.Parse(response);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public IModbusPoint MbPoint
        {
            get { return this.mbPoint; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] ReadResponse
        {
            get { return this.readResponse; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] WriteRequest
        {
            get { return this.writeResponse; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object[] ReadData
        {
            set { this.readData = value; }
        }

        #endregion

        #endregion
    }
}
