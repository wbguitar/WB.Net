using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using WB.IIIParty.Commons.Net.Sockets;
using WB.IIIParty.Commons.Protocol;
using WB.IIIParty.Commons.Logger;

namespace WB.IIIParty.Commons.Net.Sockets
{
    #region Enum
    /// <summary>
    /// Enumera i server gestiti dalla connessione
    /// </summary>
    public enum Servers 
    { 
        /// <summary>
        /// Server primario
        /// </summary>
        Primary=0,
        /// <summary>
        /// Server secondario
        /// </summary>
        Sencodary = 1
    }    
    #endregion

    #region Server Information
    /// <summary>
    /// Contiene le informazioni di un server
    /// </summary>
    public struct ServerInfo
    {
        /// <summary>
        /// Tipologia di server
        /// </summary>
        public Servers Server;
        /// <summary>
        /// Indirizzo ip
        /// </summary>
        public string IpAddress;
        /// <summary>
        /// Porta
        /// </summary>
        public int Port;
    }
    #endregion

    #region Delegate
    /// <summary>
    /// Delega la notifica della connessione avvenuta ad uno dei server
    /// </summary>
    public delegate void ConnectEvent();
    /// <summary>
    /// Delega la notifica della disconnessione da tutti e due i server
    /// </summary>
    public delegate void DisconnectEvent();
    /// <summary>
    /// Delega la notifica della connessione avvenuta ad un server
    /// </summary>
    public delegate void ServerConnectEvent(ServerInfo server);
    /// <summary>
    /// Delaga la notifica della disconnessione da un server
    /// </summary>
    public delegate void ServerDisconnectEvent(ServerInfo server);
    #endregion

    /// <summary>
    /// Classe per la definizione del client ridondante
    /// </summary>
    public class RedundantClientTcp : IProtocol, IDisposable
    {
        #region Protected Attribute 

        private bool currentConnected = false;
        /// <summary>
        /// Log
        /// </summary>
        private ITraceLog trLog = null;
        /// <summary>
        /// Stream parser per l'inizializzazione del client
        /// </summary>
        private StreamParser strParserPrim;
        /// <summary>
        /// Stream parser per l'inizializzazione del client
        /// </summary>
        private StreamParser strParserSec;
        /// <summary>
        /// Client principale
        /// </summary>
        protected TcpClient clientPrinc;
        /// <summary>
        /// Client secondario
        /// </summary>
        protected TcpClient clientSec;
        /// <summary>
        /// Struttura che memorizza i dati dei server primario
        /// </summary>
        protected ServerInfo infoServerPrimary;
        /// <summary>
        /// Struttura che memorizza i dati dei server secondario
        /// </summary>
        protected ServerInfo infoServerSecondary;
        /// <summary>
        /// Stato di connessione client primario
        /// </summary>
        bool primaryConnected;
        /// <summary>
        /// Stato di connessione client secondario
        /// </summary>
        bool secondaryConnected;
        /// <summary>
        /// 
        /// </summary>
        private bool isRedundant = false;
        #endregion

        #region Event
        /// <summary>
        /// Notifica la connessione avvenuta ad almeno uno dei server
        /// </summary>
        public event ConnectEvent OnConnect;
        /// <summary>
        /// Notifica la disconnessione da tutti e due i server
        /// </summary>
        public event DisconnectEvent OnDisconnect;
        /// <summary>
        /// Notifica la connessione avvenuta ad un server
        /// </summary>
        public event ServerConnectEvent OnServerConnect;
        /// <summary>
        /// Notifica la disconnessione da un server
        /// </summary>
        public event ServerDisconnectEvent OnServerDisconnect;
        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore di definizione
        /// </summary>
        /// <param name="primaryIpAddress">Indirizzo ip del server primario</param>
        /// <param name="primaryPort">Porta del server primario</param>
        /// <param name="reconnectScanTime">Tempo di riconnessione</param>
        /// <param name="tr">Trace logger</param>
        /// <param name="_parser">Interfaccia di parsing dei messaggi</param>
        /// <param name="_syncTimeout">Timeout di invio messaggi in modalità sincrona</param>
        public RedundantClientTcp(string primaryIpAddress, int primaryPort, TimeSpan reconnectScanTime, ITraceLog tr,
            IMessageParser _parser,TimeSpan _syncTimeout)
        {
            isRedundant = false;

            // Inserimento dei dati del server primario
            this.infoServerPrimary.Server = Servers.Primary;
            this.infoServerPrimary.IpAddress = primaryIpAddress;
            this.infoServerPrimary.Port = primaryPort;
            // Inserimento dei dati del server secondario
            this.infoServerSecondary.Server = Servers.Sencodary;
            this.infoServerSecondary.IpAddress = "";
            this.infoServerSecondary.Port = 0;
            // Assegnamento trace log
            this.trLog = tr;
            // Inizializzazione dei client
            this.clientPrinc = new TcpClient(primaryIpAddress, primaryPort, reconnectScanTime, true);
            
            // Segnature agli eventi            
            this.clientPrinc.OnConnect += new TcpClient.ConnectEvent(clientPrinc_OnConnect);
            this.clientPrinc.OnDisconnect += new TcpClient.DisconnectEvent(clientPrinc_OnDisconnect);
            this.clientPrinc.OnTrace += new dOnTrace(clientPrinc_OnTrace);

            this.strParserPrim = new StreamParser(this.clientPrinc, _parser, _syncTimeout);
            this.strParserPrim.MessageReceived += new dMessageReceived(strParserPrim_MessageReceived);
            this.strParserPrim.OnMessageParseError += new dOnMessageParseError(strParserPrim_OnMessageParseError);            
        }

        /// <summary>
        /// Costruttore di definizione
        /// </summary>
        /// <param name="primaryIpAddress">Indirizzo ip del server primario</param>
        /// <param name="primaryPort">Porta del server primario</param>
        /// <param name="secondaryIpAddress">Indirizzo ip del server secondario</param>
        /// <param name="secondaryPort">Porta del server secondario</param>
        /// <param name="reconnectScanTime">Tempo di riconnessione</param>
        /// <param name="tr">Trace logger</param>
        /// <param name="_parser">Parser dei messaggi di protocollo</param>
        /// <param name="_syncTimeout">Timeout di esecuzione dei messaggi in modlaità sincrona</param>
        public RedundantClientTcp(string primaryIpAddress, int primaryPort, 
            string secondaryIpAddress, int secondaryPort, TimeSpan reconnectScanTime, ITraceLog tr,
            IMessageParser _parser, TimeSpan _syncTimeout)
        {
            isRedundant = true;

            // Inserimento dei dati del server primario
            this.infoServerPrimary.Server = Servers.Primary;
            this.infoServerPrimary.IpAddress = primaryIpAddress;
            this.infoServerPrimary.Port = primaryPort;
            // Inserimento dei dati del server secondario
            this.infoServerSecondary.Server = Servers.Sencodary;
            this.infoServerSecondary.IpAddress = secondaryIpAddress;
            this.infoServerSecondary.Port = secondaryPort;
            // Assegnamento trace log
            this.trLog = tr;
            // Inizializzazione dei client
            this.clientPrinc = new TcpClient(primaryIpAddress, primaryPort, reconnectScanTime,true);
            this.clientSec = new TcpClient(secondaryIpAddress, secondaryPort, reconnectScanTime,true);       
            // Segnature agli eventi            
            this.clientPrinc.OnConnect += new TcpClient.ConnectEvent(clientPrinc_OnConnect);
            this.clientPrinc.OnDisconnect += new TcpClient.DisconnectEvent(clientPrinc_OnDisconnect);
            this.clientPrinc.OnTrace += new dOnTrace(clientPrinc_OnTrace);
            this.clientSec.OnConnect += new TcpClient.ConnectEvent(clientSec_OnConnect);
            this.clientSec.OnDisconnect += new TcpClient.DisconnectEvent(clientSec_OnDisconnect);
            this.clientSec.OnTrace += new dOnTrace(clientSec_OnTrace);

            this.strParserPrim = new StreamParser(this.clientPrinc, _parser, _syncTimeout);
            this.strParserPrim.MessageReceived += new dMessageReceived(strParserPrim_MessageReceived);
            this.strParserPrim.OnMessageParseError += new dOnMessageParseError(strParserPrim_OnMessageParseError);

            this.strParserSec = new StreamParser(this.clientSec, _parser, _syncTimeout);
            this.strParserSec.MessageReceived += new dMessageReceived(strParserSec_MessageReceived);
            this.strParserSec.OnMessageParseError += new dOnMessageParseError(strParserSec_OnMessageParseError);   
        }
       
        #endregion

        #region Properties
        /// <summary>
        /// Ritorna lo stato di connessione ad un server (stato di connesso = 1 solo server connesso)
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return currentConnected;
            }
        }
        /// <summary>
        /// Ritorna lo stato di connessione al server primario
        /// </summary>
        public bool IsPrimaryConnected
        {
            get
            {
                if (this.clientPrinc.IsConnected)
                { 
                    return true; 
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Ritorna se abilitata la ridondanza di client
        /// </summary>
        public bool IsRedundant
        {
            get
            {
                return isRedundant;                
            }
        }
        /// <summary>
        /// Ritorna lo stato di connessione al server secondarion
        /// </summary>
        public bool IsSecondaryConnected
        {
            get
            {
                if ((this.IsRedundant) && (this.clientSec.IsConnected))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Ritorna le informazioni del server primary
        /// </summary>
        public ServerInfo InfoServerPrimary
        {
            get { return this.infoServerPrimary; }
        }
        /// <summary>
        /// Ritorna le informazioni del server secondary
        /// </summary>
        public ServerInfo InfoServerSecondary
        {
            get { return this.infoServerSecondary; }
        }
        /// <summary>
        /// Ritorna l'interfaccia stream del primo client tcp
        /// </summary>
        public IStream PrimaryStream
        {
            get
            {
                return clientPrinc;
            }
        }
        /// <summary>
        /// Ritorna l'interfaccia stream del secondo client tcp
        /// </summary>
        public IStream SecondaryStream
        {
            get
            {
                return clientSec;
            }
        }

        #endregion

        #region Public Method
        /// <summary>
        /// Metodo per la disconnessione dei client
        /// </summary>
        public void Disconnect()
        {
            this.clientPrinc.Disconnect();

            if (this.IsRedundant)
            {
                this.clientSec.Disconnect();
            }

            EvaluateConnectionState();
        }
        /// <summary>
        /// Metodo per la chiusura della connessione che non termina il thread di riconnessione
        /// </summary>
        public void ForceDisconnect()
        {
            if (clientPrinc.IsConnected)
            {
                this.clientPrinc.CloseConnection();
            }

            if ((this.IsRedundant) && (clientSec.IsConnected))
            {
                this.clientSec.CloseConnection();
            }
        }
        /// <summary>
        /// Metodo per la connessione dei client
        /// </summary>
        public void Connect()
        {
            TimeSpan interval = new TimeSpan(0);
            this.clientPrinc.Connect(interval);
            if (this.IsRedundant) this.clientSec.Connect(interval);

            EvaluateConnectionState();
        }

        #endregion

        #region Client Event
        /// <summary>
        /// Client principale connesso
        /// </summary>
        void clientPrinc_OnConnect()
        {
            if (this.OnServerConnect != null)
            {
                this.OnServerConnect(this.InfoServerPrimary);
            }

            EvaluateConnectionState();

        }          
        /// <summary>
        /// Client secondario connesso
        /// </summary>
        void clientSec_OnConnect()
        {
            if (this.OnServerConnect != null)
            {
                this.OnServerConnect(this.InfoServerSecondary);
            }

            EvaluateConnectionState();       
        }

        /// <summary>
        /// Metodo per la vzalutazione della connessione
        /// </summary>
        void EvaluateConnectionState()
        {
            lock (this)
            {
                bool _currentConnected = false;
                bool _primaryConnected = this.clientPrinc.IsConnected;
                bool _secondaryConnected = false;
                if (this.IsRedundant) _secondaryConnected = this.clientSec.IsConnected;

                primaryConnected = _primaryConnected;
                secondaryConnected = _secondaryConnected;

                if ((this.IsRedundant) & (_primaryConnected) & (_secondaryConnected)) { _currentConnected = true; }
                if (_primaryConnected != _secondaryConnected) { _currentConnected = true; }

                if (currentConnected != _currentConnected)
                {
                    currentConnected = _currentConnected;
                    if (currentConnected) { if (OnConnect != null)OnConnect(); }
                    if (!currentConnected) { if (OnDisconnect != null)OnDisconnect(); }
                }
            }
        }
        /// <summary>
        /// Client primario disconnesso
        /// </summary>
        void clientPrinc_OnDisconnect()
        {
            if (this.OnServerDisconnect != null)
            {
                this.OnServerDisconnect(this.InfoServerPrimary);
            }

            EvaluateConnectionState();
        } 
        /// <summary>
        /// Client secondario disconnesso
        /// </summary>
        void clientSec_OnDisconnect()
        {
            if (this.OnServerDisconnect != null)
            {
                this.OnServerDisconnect(this.InfoServerSecondary);
            }

            EvaluateConnectionState();
        }
        /// <summary>
        /// Evento on trace del client primario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="direction"></param>
        /// <param name="data"></param>
        /// <param name="description"></param>
        void clientPrinc_OnTrace(IStream sender, WB.IIIParty.Commons.Protocol.TraceDirections direction, byte[] data, string description)
        {
            if (this.trLog != null)
            {
                string device = string.Empty;
                try
                {
                    if (this.clientPrinc.LocalEndPoint != null)
                        device = this.clientPrinc.LocalEndPoint.ToString();
                }
                catch { }
                string remotedevice = string.Empty;
                try
                {
                    if (this.clientPrinc.RemoteEndPoint != null)
                        remotedevice = this.clientPrinc.RemoteEndPoint.ToString();
                }
                catch { }

                PrintTypeByteArray print = PrintTypeByteArray.Hexadecimal;
                WB.IIIParty.Commons.Logger.TraceDirections trace = new WB.IIIParty.Commons.Logger.TraceDirections();
                if (direction == WB.IIIParty.Commons.Protocol.TraceDirections.Input)
                {
                    trace = WB.IIIParty.Commons.Logger.TraceDirections.Input;
                }
                else
                {
                    trace = WB.IIIParty.Commons.Logger.TraceDirections.Output;
                }
                this.trLog.Log(device, remotedevice, data, trace, description, print);
            }
        }   
        /// <summary>
        /// Evento ontrace del client secondario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="direction"></param>
        /// <param name="data"></param>
        /// <param name="description"></param>
        void clientSec_OnTrace(IStream sender, WB.IIIParty.Commons.Protocol.TraceDirections direction, byte[] data, string description)
        {
            if (this.trLog != null)
            {
                string device = string.Empty;
                try
                {
                    if (this.clientSec.LocalEndPoint != null)
                        device = this.clientSec.LocalEndPoint.ToString();
                }
                catch { }
                string remotedevice = string.Empty;
                try
                {
                    if (this.clientSec.RemoteEndPoint != null)
                        remotedevice = this.clientSec.RemoteEndPoint.ToString();
                }
                catch { }

                PrintTypeByteArray print = PrintTypeByteArray.Hexadecimal;
                WB.IIIParty.Commons.Logger.TraceDirections trace = new WB.IIIParty.Commons.Logger.TraceDirections();
                if (direction == WB.IIIParty.Commons.Protocol.TraceDirections.Input)
                {
                    trace = WB.IIIParty.Commons.Logger.TraceDirections.Input;
                }
                else
                {
                    trace = WB.IIIParty.Commons.Logger.TraceDirections.Output;
                }
                this.trLog.Log(device, remotedevice, data, trace, description, print);
            }
        }
        #endregion
       
        #region Event IProtocol
        /// <summary>
        /// Ricezione dei messaggi 
        /// </summary>
        /// <param name="msg">messaggio ricevuto</param>
        /// <param name="error">eventuale eccezione</param>       
        void strParserPrim_MessageReceived(IMessage msg, MessageValidationException error)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(msg, error);
            }
        }
        /// <summary>
        /// Ricezione dei messaggi 
        /// </summary>
        /// <param name="msg">messaggio ricevuto</param>
        /// <param name="error">eventuale eccezione</param>       
        void strParserSec_MessageReceived(IMessage msg, MessageValidationException error)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(msg,error);
            }
        }
       
        /// <summary>
        /// Errore nel parse dei dati 
        /// </summary>
        /// <param name="error">errore</param>
        void strParserPrim_OnMessageParseError(MessageParseException error)
        {
            if (this.OnMessageParseError != null)
            {
                this.OnMessageParseError(error);
            }
        }
        /// <summary>
        /// Errore nel parse dei dati 
        /// </summary>
        /// <param name="error">errore</param>
        void strParserSec_OnMessageParseError(MessageParseException error)
        {
            if (this.OnMessageParseError != null)
            {
                this.OnMessageParseError(error);
            }
        }
        #endregion

        #region IProtocol Members

        /// <summary>
        /// Notifica la ricezione di un messaggio da uno dei due server
        /// </summary>         
        public event dMessageReceived MessageReceived;

        /// <summary>
        /// Errore di parsing del messaggio
        /// </summary>        
        public event dOnMessageParseError OnMessageParseError;

        /// <summary>
        /// Invia il messaggio al server connesso
        /// </summary>
        /// <param name="msg">messaggio da inviare</param>
        public void Send(IMessage msg)
        {
            if (this.IsConnected)
            {
                if (this.clientPrinc.IsConnected)
                {
                    this.strParserPrim.Send(msg);
                    return;
                }
                
                if ((this.IsRedundant) && (this.clientSec.IsConnected))
                {
                    this.strParserSec.Send(msg);
                }
                
            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Metodo per la distruzione degli oggetti
        /// </summary>
        public void Dispose()
        {
                this.clientPrinc.Dispose();

                if (this.IsRedundant) this.clientSec.Dispose();
        }

        #endregion

        #region IProtocol Members

        /// <summary>
        /// Invia un messaggio request-response in modalità sincrona
        /// </summary>
        /// <param name="msgIn">Messaggio di richiesta</param>
        /// <param name="msgOut">Messaggi di risposta</param>
        /// <param name="vEx">Eccezioni di validazione</param>
        public SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx)
        {
            msgOut = null;
            vEx = null;
            if (this.IsConnected)
            {
                if (this.clientPrinc.IsConnected)
                {
                    return this.strParserPrim.SendSync(msgIn,out msgOut,out vEx);
                }

                if ((this.IsRedundant) && (this.clientSec.IsConnected))
                {
                    return this.strParserSec.SendSync(msgIn,out msgOut,out vEx);
                }

            }
            return SyncResult.NotAvaiable;
        }

        /// <summary>
        /// Invia un messaggio request-response in modalità asincrona
        /// </summary>
        /// <param name="msg">Messaggio di richiesta</param>
        /// <param name="callback">Callback di risposta</param>
        /// <param name="extra">Include dati extra che vengono notificati alla callback</param>
        public ASyncResult SendAsync(IMessage msg, dAsyncCallback callback, object extra)
        {
            if (this.IsConnected)
            {
                if (this.clientPrinc.IsConnected)
                {
                    return this.strParserPrim.SendAsync(msg,callback,extra);
                }

                if ((this.IsRedundant) && (this.clientSec.IsConnected))
                {
                   return this.strParserSec.SendAsync(msg,callback,extra);
                }
            }
            return ASyncResult.NotAvaiable;
        }

        #endregion


        #region IProtocol Members

        /// <summary>
        /// Invia un messaggio request-response in modalità sincrona
        /// </summary>
        /// <param name="msgIn">Messaggio di richiesta</param>
        /// <param name="msgOut">Messaggi di risposta</param>
        /// <param name="vEx">Eccezioni di validazione</param>
        /// <param name="timeout">Timeout ricezione risposta</param>
        public SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx, TimeSpan timeout)
        {
            msgOut = null;
            vEx = null;
            if (this.IsConnected)
            {
                if (this.clientPrinc.IsConnected)
                {
                    return this.strParserPrim.SendSync(msgIn, out msgOut, out vEx,timeout);
                }

                if ((this.IsRedundant) && (this.clientSec.IsConnected))
                {
                    return this.strParserSec.SendSync(msgIn, out msgOut, out vEx, timeout);
                }

            }
            return SyncResult.NotAvaiable;
        }

        #endregion
    }
}
