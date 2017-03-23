// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008 
//Progetto:             AMIL5
//Autore:               Marziali Valentina
//Nome modulo software: SyncTablesCommons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using System.Data;
using System.Collections;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;
using WB.IIIParty.Commons.Data.Sql.SyncTablesCommons;
namespace WB.IIIParty.Commons.Data.Sql.NoRedundantTables
{

    /// <summary>
    /// Rappresenta un'istanza Sql
    /// </summary>
    public class SingleServerInfo
    {
        #region Const
        private const int commandTimeOutDefault = 30000; //espresso in millisecondi
        private const int timeControllerDefault = 120000; //espresso in millisecondi
        #endregion Const

        #region PublicField

        /// <summary>
        /// Nome dell'istanza del server sql 
        /// </summary>
        public string localName;

        /// <summary>
        /// Password per la connessione 
        /// </summary>
        public string password;

        /// <summary>
        /// Nome utente per la connessione
        /// </summary>
        public string userName;

        /// <summary>
        /// Nome dell'istanza del server sql di failover
        /// </summary>
        public string failoverPartner;

        /// <summary>
        /// Indica lo stato di connessione
        /// </summary>
        public ConnectionStatus status;

        /// <summary>
        /// TimeOut di esecuzione dei comandi,
        /// espresso in secondi.
        /// </summary>
        public int commandTimeOut;

        /// <summary>
        /// Intervallo di tempo tra due verifiche successive,di raggiungibilità
        /// dei server espresso in secondi.
        /// </summary>
        public int periodicTime;

        #endregion PublicField

        #region PrivateField

        private Thread ConnectionStatusController;
        private bool enableController = false;
        private IMessageLog log;

        EventWaitHandle waitHandle;
        bool isInizialized;

        /// <summary>
        /// Disabilita l'utilizzo del pool delle connessioni
        /// al database
        /// </summary>
        private Boolean pooling = false;

        #endregion PrivateField
        
        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localName">Nome dell'istanza del server sql </param>
        /// <param name="_userName">Nome utente per la connessione</param>
        /// <param name="_password">Password per la connessione </param>
        public SingleServerInfo(string _localName,
                           string _userName, string _password, string _failoverPartner):
            this(_localName,_userName,_password)
        {
            
            this.failoverPartner = _failoverPartner;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localName">Nome dell'istanza del server sql </param>
        /// <param name="_userName">Nome utente per la connessione</param>
        /// <param name="_password">Password per la connessione </param>
        public SingleServerInfo(string _localName,
                           string _userName, string _password)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localName = _localName;
            this.userName = _userName;
            this.password = _password;
            this.status = ConnectionStatus.Bad;
            this.isInizialized= false;
            this.commandTimeOut = commandTimeOutDefault;
            this.periodicTime = timeControllerDefault;
            this.failoverPartner = string.Empty;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localName">Nome dell'istanza del server sql </param>
        /// <param name="_userName">Nome utente per la connessione</param>
        /// <param name="_password">Password per la connessione </param>
        /// <param name="_commandTimeOutDefault">Timeout di connessione in secondi</param>
        public SingleServerInfo(string _localName,
                           string _userName, string _password,
                            int _commandTimeOutDefault, string _failoverPartner)
                            : this(_localName,
                           _userName, _password,
                            _commandTimeOutDefault)
        {
            this.failoverPartner = _failoverPartner;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localName">Nome dell'istanza del server sql </param>
        /// <param name="_userName">Nome utente per la connessione</param>
        /// <param name="_password">Password per la connessione </param>
        /// <param name="_commandTimeOutDefault">Timeout di connessione in secondi</param>
        public SingleServerInfo(string _localName, 
                           string _userName, string _password,
                            int _commandTimeOutDefault)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localName = _localName;
            this.userName = _userName;
            this.password = _password;
            this.status = ConnectionStatus.Bad;
            this.isInizialized= false;
            this.commandTimeOut = _commandTimeOutDefault;
            this.periodicTime = timeControllerDefault;
            this.failoverPartner = string.Empty;            
        }

        
        /// <summary>
        /// Costruttore
        /// </summary>
        public SingleServerInfo()
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localName = string.Empty;
            this.userName = string.Empty;
            this.password = string.Empty;
            this.status = ConnectionStatus.Bad;
            this.isInizialized = false;
            this.commandTimeOut = commandTimeOutDefault;
            this.periodicTime = timeControllerDefault;
            this.failoverPartner = string.Empty;
        }

        #endregion Constructor

        #region PublicMethod
        /// <summary>
        /// Abilita il processo di verifica della raggiungibilità
        /// del server.
        /// </summary>
        /// <param name="_time">Intervallo di tempo, espresso in secondi, 
        /// tra una verifica e l'altra.</param>
        public void EnableControllerConnection(uint _time)
        {
            lock (this)
            {
                this.enableController = true;
                this.waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                ConnectionStatusController = new Thread(new ThreadStart(ControllerConnection));
                ConnectionStatusController.Name = "Sette";
                ConnectionStatusController.Start();
            }
        }

        /// <summary>
        /// Disabilita il processo di verifica della raggiungibilità 
        /// del server.
        /// </summary>
        public void DisableControllerConnection()
        {
            lock (this)
            {
                if (this.enableController)
                {
                    this.enableController = false;
                }

                if ((this.ConnectionStatusController != null) && (this.ConnectionStatusController.IsAlive))
                {
                    this.ConnectionStatusController.Abort();
                    this.ConnectionStatusController.Join();
                }
                    
            }
        }


        /// <summary>
        /// Attende la fine dell'inizializzazione delgi oggetti.
        /// </summary>
        /// <param name="_timeOut">Timeout in millisecondi</param>
        /// <returns>True: se l'inizializzazione è finita prima del timeout,
        /// false altrimenti.</returns>
        public bool WaitForReady(int _timeOut)
        { 
            bool result =true;
           if (!(this.waitHandle.WaitOne(_timeOut, true)))
            {
                result = false;
                throw new TimeOutServerInfoException("Timeout Expired. Can not "+
                    "inizialize ServerInfo instance "+ this.localName+").");
            }
            return result;
        }
        /// <summary>
        /// Estrae la lista dei database appartenenti al server
        /// </summary>   
        /// <returns></returns>
        public List<NoRedundantDatabase> GetLocalDbList(string _propertySyncName,
                                     byte _syncValue)
        {
            List<NoRedundantDatabase> result = new List<NoRedundantDatabase>();
            SqlCommand cmd = new SqlCommand();
            ArrayList databases = new ArrayList();
            try
            {
                SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                conn.DataSource = this.localName;
                conn.InitialCatalog = "master";
                conn.UserID = this.userName;
                conn.Password = this.password;
                conn.Pooling = false;
                if (this.failoverPartner != string.Empty) conn.FailoverPartner = this.failoverPartner;

                cmd.Connection = new SqlConnection(conn.ConnectionString);
                cmd.CommandTimeout = this.commandTimeOut;
                cmd.CommandText = "SELECT UPPER(NAME) FROM sys.databases";
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    databases.Add(reader.GetValue(0));

                reader.Dispose();
                cmd.Connection.Close();

                cmd.Connection.Open();
                object obj = 0;

                if ((_propertySyncName != null) && (_propertySyncName != string.Empty))
                {
                    foreach (object current in databases)
                    {
                        cmd.CommandText = "USE " + current.ToString() +
                                    " SELECT value as 'ExtPropValue'" +
                                    " FROM fn_listextendedproperty('" + _propertySyncName +
                                    "', default, default, default, default, default, default)";
                        try
                        {
                            obj = cmd.ExecuteScalar();
                            if ((obj != null) && (_syncValue == byte.Parse((string)obj)))
                                result.Add(new NoRedundantDatabase(this, current.ToString()));
                        }
                        catch (Exception ex)
                        {
                            //log
                            log.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                               " - SyncServer.GetLocalDbList(1) - " + ex.Message);
                        }
                    }
                }
                else
                    foreach (object current in databases)
                    {
                        if (obj != null)
                            result.Add(new NoRedundantDatabase(this, current.ToString()));
                    }
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                   " - SyncServer.GetLocalDbList(2) - " + Ex.Message);
            }

            finally
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Open) cmd.Connection.Close();
                cmd.Dispose();

                databases.Clear();
            }

            return result;
        }
        #endregion PublicMethod

        #region PrivateMethod
       
        /// <summary>
        /// Verifica lo stato di connessione con il server.
        /// </summary>
        /// <returns></returns>
        private void ControllerConnection()
        {
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
            this.waitHandle.Reset();
            while (this.enableController)
            {
                SqlCommand cmd = new SqlCommand();

                try
                {
                    conn.DataSource = this.localName;
                    conn.InitialCatalog = "master";
                    conn.UserID = this.userName;
                    conn.Password = this.password;
                    conn.Pooling = this.pooling;
                    if (failoverPartner != string.Empty) conn.FailoverPartner = this.failoverPartner;

                    cmd.Connection = new SqlConnection(conn.ConnectionString);
                    cmd.CommandTimeout = this.commandTimeOut;
     
                  //  cmd.Connection.Open();
                    lock (this)
                    {
                        this.status = ConnectionStatus.Good;
                    }
                    if (isInizialized == false)
                    {
                        this.isInizialized = true;
                        this.waitHandle.Set();
                    }
                }

                catch (Exception Ex)
                {
                    lock (this)
                    {
                        this.status = ConnectionStatus.Bad;
                    }
                    if (isInizialized == false)
                    {
                        this.waitHandle.Set();
                        //log
                        log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                           " - ServerInfo.ControllerConnection - Impossibile connettersi al server " + this.localName + ". " +
                           Ex.Message);  
   
                        //throw new Exception("ServerInfo: can not connect to server " + this.localName + ".", Ex);
                    }
                }

                finally
                {
                    if ((cmd.Connection!=null)&& (cmd.Connection.State == System.Data.ConnectionState.Open)) cmd.Connection.Close();
                    cmd.Dispose();
                    //log
                    log.Log(LogLevels.Info, CustomTimeStamp.GetTimeStamp() +
                       " -  ServerInfo.ControllerConnection - "+
                       " Eseguito controllo connessione al server "+this.localName+". Esito: "+ this.status.ToString()
                       + " Periodo: " + this.periodicTime.ToString());
                      
                }
                Thread.Sleep(this.periodicTime);
            }
        }

        #endregion PrivateMethod

        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //G.V. cambiato metodo per disabilitare il thread ControllerConnection
            this.DisableControllerConnection();

            

            if(log!=null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion Dispose


    }// END CLASS DEFINITION ServerInfo
}