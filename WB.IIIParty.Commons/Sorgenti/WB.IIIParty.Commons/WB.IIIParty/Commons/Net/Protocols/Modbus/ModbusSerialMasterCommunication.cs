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
using SerialNET;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Utility;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus
{
    /// <summary>
    /// 
    /// </summary>
    public class ModbusSerialMasterCommunication : IModbusCommunication
    {
        #region Private Members

        #region Private Field

        /// <summary>
        /// Identificativo del Master Modbus Seriale.
        /// </summary>
        private string mbMasterSerialId;
        /// <summary>
        /// Carattere di inizio trigger
        /// </summary>
        private string StartTrigger = ":";
        /// <summary>
        /// Caratteri di fine trigger
        /// </summary>
        private string EndTrigger = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString();
        /// <summary>
        /// Frame seriale Modbus
        /// </summary>
        private IModbusSerialLineFrame mbSerialLineFrame;

        #region ModbusADU Prameters

        /// <summary>
        /// Transaction ID Modbus.
        /// </summary>
        private Int16 mbDeviceAddress;

        #endregion

        #region Serial Line Communication Parameter

        /// <summary>
        /// Oggetto che definisce la libreria seriale.
        /// </summary>
        private SerialNET.Port serialCOM;
        /// <summary>
        /// Modalità di trasmissione seriale Modbus
        /// </summary>
        private SerialLineTransmissionMode mbTransmissionMode;
        /// <summary>
        /// Porta seriale COM
        /// </summary>
        private int mbCOMPort;
        /// <summary>
        /// Baud Rate seriale Modbus
        /// </summary>
        private SerialLineBaudRate mbSerialBaudRate;
        /// <summary>
        /// Data Bits seriale Modbus
        /// </summary>
        private SerialLineDataBits mbSerialDataBits;
        /// <summary>
        /// Stop Bits seriale Modbus
        /// </summary>
        private SerialLineStopBits mbSerialStopBits;
        /// <summary>
        /// Parity seriale Modbus
        /// </summary>
        private SerialLineParity mbSerialParity;
        /// <summary>
        /// Timeout di comunicazione
        /// </summary>
        private TimeSpan mbTimeout;
        /// <summary>
        /// Stato della connessione.
        /// </summary>
        private bool isConnected;
        /// <summary>
        /// Stringa di ricezione dati
        /// </summary>
        private string stringReceived;

        #endregion

        /// <summary>
        /// Oggetto per la creazione dei messaggi di log.
        /// </summary>
        private IMessageLog messageLog;

        #endregion

        #region Private Method

        /// <summary>
        /// Configurazione dell'oggetto della libreria SerialNET.
        /// </summary>
        private void SettingSerialNET()
        {
            if (this.serialCOM != null)
            {
                this.serialCOM.WriteTimeout = 0;
                this.serialCOM.Handshake = Handshake.None;
                this.serialCOM.ComPort = this.mbCOMPort;

                switch (this.mbTransmissionMode)
                {
                    case SerialLineTransmissionMode.RTU:
                        this.mbSerialLineFrame = new ModbusSerialLineRTUFrame();
                        this.serialCOM.Timeout = Convert.ToInt32(this.mbTimeout.TotalMilliseconds);
                        break;
                    case SerialLineTransmissionMode.ASCII:
                        this.mbSerialLineFrame = new ModbusSerialLineASCIIFrame();
                        this.serialCOM.StartTrigger = this.StartTrigger;
                        this.serialCOM.EndTrigger = this.EndTrigger;
                        break;
                }
                this.serialCOM.BaudRate = Convert.ToInt32(this.mbSerialBaudRate);
                this.serialCOM.ByteSize = Convert.ToInt32(this.mbSerialDataBits);
                switch (this.mbSerialStopBits)
                {
                    case SerialLineStopBits.SB_1:
                        this.serialCOM.StopBits = StopBits.One;
                        break;
                    case SerialLineStopBits.SB_1_5:
                        this.serialCOM.StopBits = StopBits.OneAndOneHalf;
                        break;
                    case SerialLineStopBits.SB_2:
                        this.serialCOM.StopBits = StopBits.Two;
                        break;
                }
                switch (this.mbSerialParity)
                {
                    case SerialLineParity.NONE:
                        this.serialCOM.Parity = Parity.No;
                        break;
                    case SerialLineParity.ODD:
                        this.serialCOM.Parity = Parity.Odd;
                        break;
                    case SerialLineParity.EVEN:
                        this.serialCOM.Parity = Parity.Even;
                        break;
                    case SerialLineParity.MARK:
                        this.serialCOM.Parity = Parity.Mark;
                        break;
                    case SerialLineParity.SPACE:
                        this.serialCOM.Parity = Parity.Space;
                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorCode"></param>
        private void serialCOM_OnForceClose(int ErrorCode)
        {
            this.isConnected = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void serialCOM_OnRead(string data)
        {
            try
            {
                switch (this.mbTransmissionMode)
                {
                    case SerialLineTransmissionMode.RTU:
                        {
                            if (data != null)
                            {
                                // Ricezione dei dati:
                                this.stringReceived = this.stringReceived + data;
                            }
                            else
                            {
                                // Dati ricevuti:
                                byte[] frame = Port.StringToByteArray(this.stringReceived);
                                // Azzeramento della stringa di ricezione dei dati.
                                stringReceived = ""; 

                                //if (this.MessageReceived != null)
                                //{
                                //    if (frame.Length > 0)
                                //    {
                                //        try
                                //        {
                                //            byte[] frameChecked = MBSLUtil.CheckResponseReceivedOnSerialLine(frame, rtuORascii);
                                //            this.MessageReceived(frameChecked);
                                //        }
                                //    }
                                //}
                            }
                        }
                        break;
                    case SerialLineTransmissionMode.ASCII:
                        {
                            if ((data[0] == ':') && (data[data.Length - 1] == Convert.ToChar(10)))
                            {
                                byte[] frame = SerialNET.Port.StringToByteArray(data);
                                //if (this.MessageReceived != null)
                                //{
                                //    this.MessageReceived(frame);
                                //}
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Log(LogLevels.Error, "ModbusSerialMasterCommunication: - " + ex.ToString());
            }
        }
        /// <summary>
        /// Ritorna l'array con i byte scambiati di posto specularmente.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private byte[] Swap(byte[] a)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                byte temp = a[i];
                a[i] = a[a.Length - i - 1];
                a[a.Length - i - 1] = temp;
            }
            return a;
        }
  
        #endregion

        #region Logger

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
        /// Costruttore
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="transmissionMode">Modalità di trasmissione</param>
        /// <param name="comPort">Porta COM</param>
        /// <param name="baudRate">Baud Rate</param>
        /// <param name="dataBits">Data Bits</param>
        /// <param name="stopBits">Stop Bits</param>
        /// <param name="parity">Parity</param>
        /// <param name="timeout">Timeout</param>
        public ModbusSerialMasterCommunication( Int16 deviceAddress,
                                                SerialLineTransmissionMode transmissionMode,
                                                int comPort,
                                                SerialLineBaudRate baudRate,
                                                SerialLineDataBits dataBits,
                                                SerialLineStopBits stopBits,
                                                SerialLineParity parity,
                                                TimeSpan timeout)
        {
            //  Licenza per l'attivazione della libreria seriale:
            //  SerialNET.License licence = new SerialNET.License();
            //  licence.LicenseKey = "Te3lbLgjL9cHyGXQfYEBeaa6MqaSzOfNUeID";

            this.mbDeviceAddress = deviceAddress;
            this.serialCOM = new Port();
            
            this.mbTransmissionMode = transmissionMode;
            this.mbCOMPort          = comPort;
            this.mbSerialBaudRate   = baudRate;
            this.mbSerialDataBits   = dataBits;
            this.mbSerialStopBits   = stopBits;
            this.mbSerialParity     = parity;
            this.mbSerialStopBits   = stopBits;
            this.mbTimeout          = timeout;

            this.SettingSerialNET();

            this.serialCOM.OnRead += new OnRead(serialCOM_OnRead);
            this.serialCOM.OnForceClose += new OnForceClose(serialCOM_OnForceClose);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Modbus Transaction Id
        /// </summary>
        public Int16 MbDeviceAddress
        {
            get { return this.mbDeviceAddress; }
        }
        /// <summary>
        /// Messag Log
        /// </summary>
        public IMessageLog MessageLog
        {
            set { this.messageLog = value; }
        }

        #endregion

        #region Delegates & Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReceived"></param>
        public delegate void dMessageReceived(byte[] dataReceived);
        /// <summary>
        /// 
        /// </summary>
        public event dMessageReceived onMessageReceived;

        #endregion

        #region Virtual Method

        /// <summary>
        /// Ritorna lo stato di connessione della comunicazione.
        /// </summary>
        /// <returns>Stato di connessione della comunicazione.</returns>
        public virtual bool IsConnected()
        {
            return this.isConnected;
        }
        /// <summary>
        /// Ritorna il parametro Modbus Identifier.
        /// </summary>
        /// <returns>Modbus Identifier.</returns>
        public virtual string GetId()
        {
            return this.mbMasterSerialId;
        }
        /// <summary>
        /// Imposta il parametro Modbus Identifier.
        /// </summary>
        /// <param name="id">Modbus Identifier.</param>
        public virtual void SetId(string id)
        {
            this.mbMasterSerialId = id;
        }
        /// <summary>
        /// Creazione dell'unità ADU Modbus.
        /// </summary>
        /// <param name="pdu">Unità PDU Modbus</param>
        /// <returns>Unità ADU Modbus</returns>
        public virtual byte[] CreateADU(byte[] pdu)
        {
            byte[] result = new byte[1];
            try
            {
                result = this.mbSerialLineFrame.CreateFrame(this.mbDeviceAddress, pdu);
            }
            catch (Exception ex)
            {
                this.Log(LogLevels.Error, "ModbusSerialMasterCommunication: - " + ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// Invio dell'unità ADU Modbus.
        /// </summary>
        /// <param name="modbusADU">Unità ADU Modbus</param>
        public virtual void SendModbusADU(byte[] modbusADU)
        {
            try
            {
                this.serialCOM.Write(Port.ByteArrayToString(modbusADU));
            }
            catch (Exception ex)
            {
                this.Log(LogLevels.Error, "ModbusSerialMasterCommunication: - " + ex.ToString());
            }
        }
        /// <summary>
        /// Analisi dell'unità ADU Modbus
        /// </summary>
        /// <param name="adu">Unità ADU Modbus</param>
        /// <returns>Unità PDU Modbus</returns>
        public virtual byte[] ParseADU(byte[] adu)
        {
            byte[] result = new byte[1];
            try
            {
                result = this.mbSerialLineFrame.ParseFrame(adu);
            }
            catch (Exception ex)
            {
                this.Log(LogLevels.Error, "ModbusSerialMasterCommunication: - " + ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// Avvio della comunicazione.
        /// </summary>
        public virtual void StartCommunication()
        {
            try
            {
                this.serialCOM.Enabled = true;
                this.isConnected = true;
            }
            catch (Exception ex)
            {
                this.Log(LogLevels.Error, "ModbusSerialMasterCommunication: - " + ex.ToString());
            }
        }
        /// <summary>
        /// Arresto della comunicazione.
        /// </summary>
        public virtual void StopCommunication()
        {
            try
            {
                this.serialCOM.Enabled = false;
                this.serialCOM.Dispose();
                this.isConnected = false;
            }
            catch (Exception ex)
            {
                this.Log(LogLevels.Error, "ModbusSerialMasterCommunication: - " + ex.ToString());
            }
        }
        /// <summary>
        /// Evento di avvenuta ricezione dati.
        /// </summary>
        public event dDataReceived onDataReceived;

        #endregion

        #endregion
    }
}
