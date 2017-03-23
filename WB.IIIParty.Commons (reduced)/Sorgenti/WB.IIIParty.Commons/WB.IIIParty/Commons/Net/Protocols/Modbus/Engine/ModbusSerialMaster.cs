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
using System.Net;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Utility;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Engine
{
    /// <summary>
    /// Client TCP/IP per protocollo Modbus 
    /// </summary>
    public class ModbusSerialMaster
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// Nome identificativo del Master Seriale.
        /// </summary>
        private string masterName;
        /// <summary>
        /// Indirizzo del dispositivo slave.  
        /// </summary>
        private Int16 mbSlaveAddress;
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
        /// Lista dei punti Modbus configurati. 
        /// </summary>
        private Dictionary<string, ModbusPoint> mbPointList = new Dictionary<string, ModbusPoint>();

        #endregion

        #endregion

        #region Public Members

        #region Constructors

        /// <summary>
        /// Costruttore senza parametri.
        /// </summary>
        public ModbusSerialMaster(){}
        /// <summary>
        /// Costruttore con i parametri di connessione.
        /// </summary>
        /// <param name="name">Nome del dispositivo</param>
        /// <param name="deviceAddress">Indirizzo del dispositivo</param>
        /// <param name="transmissionMode">Modalità di trasmissione</param>
        /// <param name="comPort">Porta COM</param>
        /// <param name="baudRate">Baud Rate</param>
        /// <param name="dataBits">Data Bits</param>
        /// <param name="stopBits">Stop Bits</param>
        /// <param name="parity">Parity</param>
        /// <param name="timeout">Timeout</param>
        public ModbusSerialMaster(  string name,
                                    Int16 deviceAddress,
                                    SerialLineTransmissionMode transmissionMode,
                                    int comPort,
                                    SerialLineBaudRate baudRate,
                                    SerialLineDataBits dataBits,
                                    SerialLineStopBits stopBits,
                                    SerialLineParity parity,
                                    TimeSpan timeout)
        {
            this.masterName         = name;
            this.mbSlaveAddress     = deviceAddress;
            this.mbTransmissionMode = transmissionMode;
            this.mbCOMPort          = comPort;
            this.mbSerialBaudRate   = baudRate;
            this.mbSerialDataBits   = dataBits;
            this.mbSerialStopBits   = stopBits;
            this.mbSerialParity     = parity;
            this.mbSerialStopBits   = stopBits;
            this.mbTimeout          = timeout;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Nome identificativo del Master seriale.
        /// </summary>
        public string MasterName
        {
            get { return this.masterName; }
            set { this.masterName = value; }
        }
        /// <summary>
        /// Indirizzo del dispositivo slave.
        /// </summary>
        public Int16 SlaveAddress
        {
            get { return this.mbSlaveAddress; }
            set { this.mbSlaveAddress = value; }
        }
        /// <summary>
        /// Modalità di trasmissione seriale Modbus
        /// </summary>
        public SerialLineTransmissionMode TransmissionMode
        {
            get { return this.mbTransmissionMode; }
            set { this.mbTransmissionMode = value; }
        }
        /// <summary>
        /// Porta seriale COM
        /// </summary>
        public int COMPort
        {
            get { return this.mbCOMPort; }
            set { this.mbCOMPort = value; }
        }
        /// <summary>
        /// Baud Rate seriale Modbus
        /// </summary>
        public SerialLineBaudRate SerialBaudRate
        {
            get { return this.mbSerialBaudRate; }
            set { this.mbSerialBaudRate = value; }
        }
        /// <summary>
        /// Data Bits seriale Modbus
        /// </summary>
        public SerialLineDataBits SerialDataBits
        {
            get { return this.mbSerialDataBits; }
            set { this.mbSerialDataBits = value; }
        }
        /// <summary>
        /// Stop Bits seriale Modbus
        /// </summary>
        public SerialLineStopBits SerialStopBits
        {
            get { return this.mbSerialStopBits; }
            set { this.mbSerialStopBits = value; }
        }
        /// <summary>
        /// Parity seriale Modbus
        /// </summary>
        public SerialLineParity SerialParity
        {
            get { return this.mbSerialParity; }
            set { this.mbSerialParity = value; }
        }
        /// <summary>
        /// Timeout di comunicazione
        /// </summary>
        public TimeSpan Timeout
        {
            get { return this.mbTimeout; }
            set { this.mbTimeout = value; }
        }
        /// <summary>
        /// Lista dei punti Modbus configurati.
        /// </summary>
        public Dictionary<string, ModbusPoint> MbPointList
        {
            get { return this.mbPointList; }
        }

        #endregion

        #endregion
    }
}
