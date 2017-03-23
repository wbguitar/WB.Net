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

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{

    /// <summary>
    /// Rappresenta un'istanza Sql
    /// </summary>
    public class ServerInfo
    {
        #region Const
        private const int commandTimeOutDefault = 30; //espresso in secondi
        private const int timeControllerDefault = 120; //espresso in secondi
        #endregion Const

        #region PublicField

        /// <summary>
        /// Nome dell'istanza del server sql 
        /// </summary>
        public string localName;

        /// <summary>
        /// Nome dell'istanza del server collegato, utilizzato per 
        /// query distribuite
        /// </summary>
        public string localLinkedName;

        /// <summary>
        /// Password per la connessione 
        /// </summary>
        public string password;

        /// <summary>
        /// Nome utente per la connessione
        /// </summary>
        public string userName;

        
        //public bool controllerStatus ;

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
        /// <param name="_linkedName">Nome dell'istanza del server collegato, utilizzato per 
        /// query distribuite</param>
        /// <param name="_userName">Nome utente per la connessione</param>
        /// <param name="_password">Password per la connessione </param>
        public ServerInfo(string _localName, string _linkedName,
                           string _userName, string _password)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localName = _localName;
            this.localLinkedName = _linkedName;
            this.userName = _userName;
            this.password = _password;
            this.status = ConnectionStatus.Bad;
            this.isInizialized= false;
            this.commandTimeOut = commandTimeOutDefault;
            this.periodicTime = timeControllerDefault;
            
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localName">Nome dell'istanza del server sql </param>
        /// <param name="_linkedName">Nome dell'istanza del server collegato, utilizzato per 
        /// query distribuite</param>
        /// <param name="_userName">Nome utente per la connessione</param>
        /// <param name="_password">Password per la connessione </param>
        /// <param name="_commandTimeOutDefault">Timeout di connessione in secondi</param>
        public ServerInfo(string _localName, string _linkedName,
                           string _userName, string _password,
                            int _commandTimeOutDefault)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localName = _localName;
            this.localLinkedName = _linkedName;
            this.userName = _userName;
            this.password = _password;
            this.status = ConnectionStatus.Bad;
            this.isInizialized= false;
            this.commandTimeOut = _commandTimeOutDefault;
            this.periodicTime = timeControllerDefault;
            
        }

        
        /// <summary>
        /// Costruttore
        /// </summary>
        public ServerInfo()
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localName = string.Empty;
            this.localLinkedName = string.Empty;
            this.userName = string.Empty;
            this.password = string.Empty;
            this.status = ConnectionStatus.Bad;
            this.isInizialized = false;
            this.commandTimeOut = commandTimeOutDefault;
            this.periodicTime = timeControllerDefault;
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
                   this.enableController = false;

                if ((this.ConnectionStatusController != null) && (this.ConnectionStatusController.IsAlive))
                   this.ConnectionStatusController.Abort();
                    
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

                    cmd.Connection = new SqlConnection(conn.ConnectionString);
                    cmd.CommandTimeout = this.commandTimeOut;
     
                    cmd.Connection.Open();
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
            if ((this.ConnectionStatusController!=null)&&(this.ConnectionStatusController.IsAlive))
                this.ConnectionStatusController.Abort();

            if(log!=null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion Dispose


    }// END CLASS DEFINITION ServerInfo
}