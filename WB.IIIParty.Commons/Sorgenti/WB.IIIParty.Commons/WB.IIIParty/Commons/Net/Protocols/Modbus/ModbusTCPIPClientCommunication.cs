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
using WB.IIIParty.Commons.Net.Sockets;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Utility;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus
{
    /// <summary>
    /// Comunicazione Client Modbus TCP/IP 
    /// </summary>
    public class ModbusTCPIPClientCommunication : IModbusCommunication
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// Identificativo del Client Modbus TCP/IP.
        /// </summary>
        private string mbClientTcpIpId;

        #region ModbusADU Prameters

        /// <summary>
        /// Transaction ID Modbus.
        /// </summary>
        private Int16 transactionID;
        /// <summary>
        /// Protocol Id Modbus.
        /// </summary>
        private Int16 protocolID;
        /// <summary>
        /// Data Lenght Modbus.
        /// </summary>
        private Int16 dataLength;
        /// <summary>
        /// Unit Id Modbus.
        /// </summary>
        private Int16 unitID;

        #endregion

        #region TCP/IP Communication Parameter

        /// <summary>
        /// Client TCP/IP.
        /// </summary>
        private TcpClient client;
        /// <summary>
        /// Indirizzo IP del server TCP/IP.
        /// </summary>
        private string ipAddress;
        /// <summary>
        /// Port del server TCP/IP.
        /// </summary>
        private int tcpPort;
        /// <summary>
        /// Intervallo di riconnessione.
        /// </summary>
        private TimeSpan reconnectionScanTime;
        /// <summary>
        /// Ritardo di connesssione.
        /// </summary>
        private TimeSpan connectionDelay;
        /// <summary>
        /// Stato della connessione.
        /// </summary>
        private bool isConnected;

        #endregion

        /// <summary>
        /// Oggetto per la creazione dei messaggi di log.
        /// </summary>
        private IMessageLog messageLog;

        #endregion

        #region Private Method

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
        /// <summary>
        /// Gestione dell'evento di connessione fallita.
        /// </summary>
        private void client_ConnectFailure()
        {
            try
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - Connection failed");
                this.isConnected = false;
                if (this.onConnectFailure != null)
                {
                    this.onConnectFailure(this);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }
        /// <summary>
        /// Gestione dell'evento di avvenuta ricezionedei dati.
        /// </summary>
        /// <param name="data"></param>
        private void client_DataReceived(byte[] data)
        {
            try
            {
                string adu = string.Empty;
                for (int i = 0; i < data.Length; i++)
                {
                    adu = " " + data[i].ToString();
                }
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - Data received :" + adu);
                if (this.onDataReceived != null)
                {
                    this.onDataReceived(this, data);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }
        /// <summary>
        /// Gestione dell'evento di avvenuta connessione
        /// </summary>
        private void client_OnConnect()
        {
            try
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - Connection established");
                this.isConnected = true;
                if (this.onConnect != null)
                {
                    this.onConnect(this);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }
        /// <summary>
        /// Gestione dell'evento di avvenuta disconnessione.
        /// </summary>
        private void client_OnDisconnect()
        {
            try
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - Disconnection");
                this.isConnected = false;
                if (this.onDisconnect != null)
                {
                    this.onDisconnect(this);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }
        /// <summary>
        /// Gestione dell'evento di OnTrace
        /// </summary>
        /// <param name="sender">Mittente</param>
        /// <param name="direction">Direzione del messaggio</param>
        /// <param name="data">Messaggio</param>
        /// <param name="description">Descrizione</param>
        private void client_OnTrace(WB.IIIParty.Commons.Protocol.IStream sender, WB.IIIParty.Commons.Protocol.TraceDirections direction, byte[] data, string description)
        {
            try
            {
                string trace = string.Empty;
                for (int i = 0; i < data.Length; i++)
                {
                    trace = " " + data[i].ToString();
                }
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - Sender: " + sender.ToString() + 
                                                                      " - Direction: " + direction.ToString() + 
                                                                      " - Data: " + trace + 
                                                                      " - Description: " + description);
                if (this.onTrace != null)
                {
                    this.onTrace(sender, direction, data, description);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
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

        #region Public Properties

        /// <summary>
        /// Modbus Transaction Id
        /// </summary>
        public Int16 TransactionID
        {
            get { return this.transactionID; }
        }
        /// <summary>
        /// Modbus Protocol Id
        /// </summary>
        public Int16 ProtocolID
        {
            get { return this.protocolID; }
            set { this.protocolID = value; }
        }
        /// <summary>
        /// Modbus Data Length
        /// </summary>
        public Int16 DataLength
        {
            get { return dataLength; }
        }
        /// <summary>
        /// Modbus Unit Id
        /// </summary>
        public Int16 UnitID
        {
            get { return unitID; }
            set { this.unitID = value; }
        }
        /// <summary>
        /// Messag Log
        /// </summary>
        public IMessageLog MessageLog
        {
            set{this.messageLog = value;}
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="ipServerAddr">Indirizzo IP del server</param>
        /// <param name="port">Porta del server</param>
        /// <param name="slaveID">Modbus Slave Id</param>
        /// <param name="reconnScanTime">Tempo di riconnessione</param>
        /// <param name="connDelay">Ritardo di connessione</param>
        public ModbusTCPIPClientCommunication(string ipServerAddr, int port, Int16 slaveID, TimeSpan reconnScanTime, TimeSpan connDelay)
        {
            this.ipAddress = ipServerAddr;
            this.tcpPort = port;
            this.reconnectionScanTime = reconnScanTime;
            this.connectionDelay = connDelay;
            this.unitID = slaveID;
            this.isConnected = false;
        }

        #endregion

        #region Public Events & Delegates

        /// <summary>
        /// Connessione fallita.
        /// </summary>
        public delegate void dConnectFailure(ModbusTCPIPClientCommunication sender);
        /// <summary>
        /// Evento di connessione fallita.
        /// </summary>
        public event dConnectFailure onConnectFailure;
        /// <summary>
        /// Evento di avvenuta ricezione dati.
        /// </summary>
        public event dDataReceived onDataReceived;
        /// <summary>
        /// Connessione
        /// </summary>
        public delegate void dOnConnect(IModbusCommunication sender);
        /// <summary>
        /// Evento di avvenuta connessione
        /// </summary>
        public event dOnConnect onConnect;
        /// <summary>
        /// Disconnessione
        /// </summary>
        public delegate void dOnDisconnect(IModbusCommunication sender);
        /// <summary>
        /// Evento di avvenuta disconnessione
        /// </summary>
        public event dOnDisconnect onDisconnect;
        /// <summary>
        /// Trace
        /// </summary>
        public delegate void dOnTrace(WB.IIIParty.Commons.Protocol.IStream sender, WB.IIIParty.Commons.Protocol.TraceDirections direction, byte[] data, string description);
        /// <summary>
        /// Evento di Trace
        /// </summary>
        public event dOnTrace onTrace;

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
            return this.mbClientTcpIpId;
        }
        /// <summary>
        /// Imposta il parametro Modbus Identifier.
        /// </summary>
        /// <param name="id">Modbus Identifier.</param>
        public virtual void SetId(string id)
        {
            this.mbClientTcpIpId = id;
        }
        /// <summary>
        /// Creazione dell'unità ADU Modbus.
        /// </summary>
        /// <param name="pdu">Unità PDU Modbus</param>
        /// <returns>Unità ADU Modbus</returns>
        public virtual byte[] CreateADU(byte[] pdu)
        {
            byte[] result = null;
            try
            {
                byte[] _transID = new byte[2];
                byte[] _protID = new byte[2];
                byte[] _length = new byte[2];
                byte[] _unitID = new byte[1];

                this.transactionID = (Int16)TransactionIDGenerator.Istance.TransacID;
                _transID = BitConverter.GetBytes(this.transactionID);
                Swap(_transID);
                _length = BitConverter.GetBytes((Int16)(pdu.Length + 1));
                Swap(_length);
                byte[] temp = BitConverter.GetBytes(this.unitID);
                _unitID[0] = temp[0];

                int size = pdu.Length + 7;
                result = new byte[size];
                int index = 0;
                Array.Copy(_transID, 0, result, 0, _transID.Length);
                index += _transID.Length;
                Array.Copy(_protID, 0, result, index, _protID.Length);
                index += _protID.Length;
                Array.Copy(_length, 0, result, index, _length.Length);
                index += _length.Length;
                Array.Copy(_unitID, 0, result, index, _unitID.Length);
                index += _unitID.Length;
                Array.Copy(pdu, 0, result, index, pdu.Length);
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
            return result;
        }
        /// <summary>
        /// Analisi dell'unità ADU Modbus
        /// </summary>
        /// <param name="adu">Unità ADU Modbus</param>
        /// <returns>Unità PDU Modbus</returns>
        public virtual byte[] ParseADU(byte[] adu)
        {
            byte[] result = null;
            try
            {
                byte[] transID = new byte[2];
                byte[] protID = new byte[2];
                byte[] length = new byte[2];
                byte[] slaveID = new byte[1];

                transID[0] = adu[1];
                transID[1] = adu[0];

                protID[0] = adu[3];
                protID[1] = adu[2];
                this.protocolID = BitConverter.ToInt16(protID, 0);
                length[0] = adu[5];
                length[1] = adu[4];
                this.dataLength = BitConverter.ToInt16(length, 0);
                slaveID[0] = adu[6];
                this.unitID = (Int16)slaveID[0];
                int size = adu.Length - 7;
                result = new byte[size];
                Array.Copy(adu, 7, result, 0, result.Length);
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
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
                client = new TcpClient(this.ipAddress, this.tcpPort, this.reconnectionScanTime);
                client.Connect(connectionDelay);
                client.ConnectFailure += new TcpClient.DisconnectEvent(client_ConnectFailure);
                client.DataReceived += new WB.IIIParty.Commons.Protocol.dDataReceived(client_DataReceived);
                client.OnConnect += new TcpClient.ConnectEvent(client_OnConnect);
                client.OnDisconnect += new TcpClient.DisconnectEvent(client_OnDisconnect);
                client.OnTrace += new WB.IIIParty.Commons.Protocol.dOnTrace(client_OnTrace);
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }
        /// <summary>
        /// Arresto della comunicazione.
        /// </summary>
        public virtual void StopCommunication()
        {
            try
            {
                client.Disconnect();
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }
        /// <summary>
        /// Invio dell'unità ADU Modbus.
        /// </summary>
        /// <param name="modbusADU">Unità ADU Modbus</param>
        public virtual void SendModbusADU(byte[] modbusADU)
        {
            try
            {
                client.Send(modbusADU);
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusTCPIPClientCommunication: - " + e.ToString());
            }
        }

        #endregion

        #endregion
    }
}
