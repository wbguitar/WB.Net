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
    /// Richiesta Modbus
    /// </summary>
    public class ModbusRequest
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
        private byte[] readRequest;
        /// <summary>
        /// 
        /// </summary>
        private byte[] writeRequest;
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
                                        //  Richiesta di lettura:
                                        this.readRequest = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.Write:
                                    {
                                        //  WriteMultipleCoils
                                        functionCode = 15;
                                        //  Richiesta di scrittura:
                                        this.writeRequest = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.ReadWrite:
                                    {
                                        //  ReadCoils:
                                        functionCode = 1;
                                        //  Richiesta di lettura:
                                        this.readRequest = this.GetByteArray(functionCode);

                                        //  WriteMultipleCoils
                                        functionCode = 15;
                                        //  Richiesta di scrittura:
                                        this.writeRequest = this.GetByteArray(functionCode);
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
                                        //  Richiesta di lettura:
                                        this.readRequest = this.GetByteArray(functionCode);
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
                                        //  Richiesta di lettura:
                                        this.readRequest = this.GetByteArray(functionCode);
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
                                        //  Richiesta di lettura:
                                        this.readRequest = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.Write:
                                    {
                                        //  WriteMultipleRegister
                                        functionCode = 16;
                                        //  Richiesta di scrittura:
                                        this.writeRequest = this.GetByteArray(functionCode);
                                    }
                                    break;
                                case ModbusAccessMode.ReadWrite:
                                    {
                                        //  HoldingRegister:
                                        functionCode = 3;
                                        //  Richiesta di lettura:
                                        this.readRequest = this.GetByteArray(functionCode);
                                        //  WriteMultipleRegister
                                        functionCode = 16;
                                        //  Richiesta di scrittura:
                                        this.writeRequest = this.GetByteArray(functionCode);
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusRequest: - " + e.ToString());
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
                System.Reflection.ConstructorInfo ci = (System.Reflection.ConstructorInfo)mbSettings.ModbusCommandType[functionCode];
                object[] param = new object[0];
                object obj = ci.Invoke(param);
                ModbusPDU pdu = (ModbusPDU)obj;
                pdu.MbParseReqPDU(message, ref this.mbPoint);
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusRequest: - " + e.ToString());
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        private byte[] GetByteArray(int functionCode)
        {
            try
            {
                System.Reflection.ConstructorInfo ci = (System.Reflection.ConstructorInfo)mbSettings.ModbusCommandType[functionCode];
                object[] param = new object[0];
                object obj = ci.Invoke(param);
                ModbusPDU pdu = (ModbusPDU)obj;
                return pdu.MbCreateReqPDU(this.mbPoint);
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusRequest: - " + e.ToString());
                return new byte[1];
            }
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
        public ModbusRequest(IModbusPoint point, IMessageLog msgLog)
        {
            this.messageLog = msgLog;
            this.mbPoint = point;
            this.readRequest    = new byte[1] { 0 };
            this.writeRequest   = new byte[1] { 0 };
            this.Create();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="msgLog"></param>
        public ModbusRequest(byte[] request, IMessageLog msgLog)
        {
            this.messageLog = msgLog;
            this.Parse(request);
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
        public byte[] ReadRequest
        {
            get 
            {
                this.Create();
                return this.readRequest; 
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] WriteRequest
        {
            get 
            {
                this.Create();
                return this.writeRequest; 
            }
        }
        
        #endregion

        #endregion
    }
}
