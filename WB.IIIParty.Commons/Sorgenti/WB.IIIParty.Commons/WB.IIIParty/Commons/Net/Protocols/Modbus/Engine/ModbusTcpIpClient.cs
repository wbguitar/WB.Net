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

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Engine
{
    /// <summary>
    /// Client TCP/IP per protocollo Modbus 
    /// </summary>
    public class ModbusTcpIpClient
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// Nome identificativo del Client TCP/IP.
        /// </summary>
        private string clientName;
        /// <summary>
        /// Indirizzo del Server Modbus TCP/IP a cui connettersi.  
        /// </summary>
        private IPAddress serverIpAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
        /// <summary>
        /// Porta del Server Modbus TCP/IP a cui connettersi.
        /// </summary>
        private int serverPort = 502;
        /// <summary>
        /// Intervallo di tempo che intercorre tra ogni tentativo di riconnessione (sec).
        /// </summary>
        private TimeSpan reconnectScanTime = new TimeSpan(0, 0, 2);
        /// <summary>
        /// Tempo che definisce il ritardo massimo di connessione (sec).
        /// </summary>
        private TimeSpan connectionDelay = new TimeSpan(0, 0, 1);
        /// <summary>
        /// Identificativo del dispositivo.
        /// </summary>
        private int  deviceId = 1;
        /// <summary>
        /// Timeout per la ricezione delle risposte (sec).
        /// </summary>
        private TimeSpan timeOut = new TimeSpan(0, 0, 1);
        /// <summary>
        /// Intervallo di tempo che intercorre tra ogni invio di richiesta (sec).
        /// </summary>
        private TimeSpan scanTime = new TimeSpan(0, 0, 1);
        /// <summary>
        /// Lista dei punti Modbus configurati. 
        /// </summary>
        private Dictionary<string, IModbusPoint> mbPointList = new Dictionary<string, IModbusPoint>();

        #endregion

        #endregion

        #region Public Members

        #region Constructors

        /// <summary>
        /// Costruttore senza parametri.
        /// </summary>
        public ModbusTcpIpClient(){}
        /// <summary>
        /// Costruttore con i parametri di connessione.
        /// </summary>
        /// <param name="name">Nome identificativo del Client TCP/IP.</param>
        /// <param name="svrIpAddr">Indirizzo del Server Modbus TCP/IP a cui connettersi.</param>
        /// <param name="svrPort">Porta del Server Modbus TCP/IP a cui connettersi.</param>
        /// <param name="reconnScanTime">Intervallo di tempo che intercorre tra ogni tentativo di riconnessione (sec).</param>
        /// <param name="connDelay">Tempo che definisce il ritardo massimo di connessione (sec).</param>
        /// <param name="timeout">Timeout per la ricezione delle risposte (sec).</param>
        /// <param name="scantime">Intervallo di tempo che intercorre tra ogni invio di richiesta (sec).</param>
        /// <param name="devId">Identificativo del dispositivo.</param>
        public ModbusTcpIpClient(   string name, 
                                    IPAddress svrIpAddr, 
                                    int svrPort, 
                                    TimeSpan reconnScanTime, 
                                    TimeSpan connDelay, 
                                    TimeSpan timeout, 
                                    TimeSpan scantime, 
                                    int devId)
        {
            this.clientName         = name;
            this.serverIpAddress    = svrIpAddr;
            this.serverPort         = svrPort;
            this.reconnectScanTime  = reconnScanTime;
            this.connectionDelay    = connDelay;
            this.timeOut            = timeout;
            this.scanTime           = scantime;
            this.deviceId           = devId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Nome identificativo del Client TCP/IP.
        /// </summary>
        public string ClientName
        {
            get { return this.clientName; }
            set { this.clientName = value; }
        }
        /// <summary>
        /// Indirizzo del Server Modbus TCP/IP a cui connettersi.
        /// </summary>
        public IPAddress ServerIpAddress
        {
            get { return this.serverIpAddress; }
            set { this.serverIpAddress = value; }
        }
        /// <summary>
        /// Porta del Server Modbus TCP/IP a cui connettersi.
        /// </summary>
        public int ServerPort
        {
            get { return this.serverPort; }
            set { this.serverPort = value; }
        }
        /// <summary>
        /// Intervallo di tempo che intercorre tra ogni tentativo di riconnessione (sec).
        /// </summary>
        public TimeSpan ReconnectScanTime
        {
            get { return this.reconnectScanTime; }
            set { this.reconnectScanTime = value; }
        }
        /// <summary>
        /// Tempo che definisce il ritardo massimo di connessione (sec).
        /// </summary>
        public TimeSpan ConnectionDelay
        {
            get { return this.connectionDelay; }
            set { this.connectionDelay = value; }
        }
        /// <summary>
        /// imeout per la ricezione delle risposte (sec).
        /// </summary>
        public TimeSpan TimeOut
        {
            get { return this.timeOut; }
            set { this.timeOut = value; }
        }
        /// <summary>
        /// Intervallo di tempo che intercorre tra ogni invio di richiesta (sec).
        /// </summary>
        public TimeSpan ScanTime
        {
            get { return this.scanTime; }
            set { this.scanTime = value; }
        }
        /// <summary>
        /// Identificativo del dispositivo.
        /// </summary>
        public int DeviceId
        {
            get { return this.deviceId; }
            set { this.deviceId = value; }
        }
        /// <summary>
        /// Lista dei punti Modbus configurati.
        /// </summary>
        public Dictionary<string, IModbusPoint> MbPointList
        {
            get { return this.mbPointList; }
            set { this.mbPointList = value; }
        }

        #endregion

        #endregion
    }
}
