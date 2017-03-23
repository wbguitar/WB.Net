// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina
//Nome modulo software: SyncTables.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System.Collections.Generic;
using WB.IIIParty.Commons.Data.Sql.SyncTablesCommons;
using System;
using System.Threading;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;

namespace WB.IIIParty.Commons.Data.Sql.SyncTables
{
   
    /// <summary>
    /// Gestisce la sincronizzazione di tabelle.
    /// </summary>
    public class SyncTables: IDisposable
    {
        #region PublicField
        
        /// <summary>
        /// TRUE se il processo di sincronizzazione è periodico, FALSE altrimenti
        /// </summary>
        public bool periodic;

        /// <summary>
        /// TRUE se il processo di sincronizzazione è attivo, FALSE altrimenti
        /// </summary>
        public bool enable;

        /// <summary>
        /// Dati di configurazione.
        /// </summary>
        public ConfigurationInfo configData;
        
        #endregion PublicField

        #region PrivateField
        
        /// <summary>
        /// 
        /// </summary>
        public List<Table> tableList = new List<Table>();
        private IMessageLog log;

        //private uint intervalSync;

        private uint intervalControllerConnection = 60; //periodo di tempo per il controllo   
                                                        //della connessione con i due server
                                                        //espresso in secondi

        private List<Database> DbList = new List<Database>();

        private SyncServers servers;

        #endregion PrivateField

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_periodic">TRUE se il processo di sincronizzazione è periodico, FALSE altrimenti</param>
        /// <param name="_enable">TRUE se il processo di sincronizzazione è attivo, FALSE altrimenti</param>
        /// <param name="_configData">Dati di configurazione</param>
        public SyncTables(bool _periodic,bool _enable,
                            ConfigurationInfo _configData)
        {
            try
            {
                log = LoggerToFile.LoggerToFile_singleton.GetLogger();
                this.periodic = _periodic;
                this.enable = _enable;
                this.configData = _configData;

                //Creo i server e la lista dei database 
                this.servers = new SyncServers(configData.localServer, configData.remoteServer);
                this.servers.localServer.EnableControllerConnection(this.intervalControllerConnection);
                this.servers.remoteServer.EnableControllerConnection(this.intervalControllerConnection);
                
                this.servers.localServer.WaitForReady(40000);
                this.DbList = servers.GetLocalDbList(this.configData.extPropertyNameSync, 1);
                TableType currentType;

                if (this.periodic) currentType = TableType.History;
                else currentType = TableType.OnStart;

                //Creo lista tabelle
                List<Table> tmpTables = new List<Table>();
                
                foreach (Database currentDb in this.DbList)
                {
                    currentDb.SyncTable.name = this.configData.tableSyncName;
                    currentDb.SyncTable.colTable = this.configData.tableSyncColumnTableName;
                    currentDb.SyncTable.colDateTimeInsert = this.configData.tableSyncColumnDateTimeNameInsert;
                    currentDb.SyncTable.colDateTimeUpdate = this.configData.tableSyncColumnDateTimeNameUpdate;

                    tmpTables = new List<Table>(currentDb.GetLocalTableList(this.configData.extPropertyNameSync,
                        1, configData.extPropertyNameType, TableType.OnEnable, configData.extPropertyLastUpdate,
                        configData.extPropertyNameIsDeleted));
                    if (tmpTables != null) this.tableList.AddRange(tmpTables);

                    tmpTables = currentDb.GetLocalTableList(this.configData.extPropertyNameSync,
                       1, configData.extPropertyNameType, currentType, configData.extPropertyLastUpdate,
                       configData.extPropertyNameIsDeleted);

                    if (tmpTables != null) this.tableList.AddRange(tmpTables);
                }
                
                tableList.Sort(new TableCompare());
                
            }
            catch (TimeOutServerInfoException Ex)
            {
                throw new TimeOutServerInfoException("SyncTables "+Ex.errorMessage);
            }
            catch (Exception Ex)
            {
                throw new Exception("", Ex);
            }
        }
        #endregion Constructor

        #region PublicMethod

        /// <summary>
        /// Abilita il processo di sincronizzazione
        /// </summary>
        public void SynchronizeTables()
        {
            if (this.periodic) this.PeriodicSync();
            else OnceSync();
        }
  
        #endregion PublicMethod

        #region PrivateMethod
        private void PeriodicSync()
        {
            while (this.enable)
            {
                
                foreach (Table currentTable in this.tableList)
                {
                    try
                    {
                        log.Log(LogLevels.Info, CustomTimeStamp.GetTimeStamp() +
                       " - SyncTables.PeriodicSync - Tabella :" + currentTable.Name +
                        " inizio sincronizzazione: " + CustomTimeStamp.GetTimeStamp());

                        if (enable) currentTable.Synchronize(this.periodic, this.configData.maxRows, false);

                        log.Log(LogLevels.Info, CustomTimeStamp.GetTimeStamp() +
                      " - SyncTables.PeriodicSync - Tabella :" + currentTable.Name +
                       " fine sincronizzazione: " + CustomTimeStamp.GetTimeStamp());
                    }
                    catch (Exception Ex)
                    {
                        //log
                        log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                           " - SyncTables.PeriodicSync (InsertUpdate) - " + Ex.Message);
                    }
                }
                for (int i = this.tableList.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        log.Log(LogLevels.Info, CustomTimeStamp.GetTimeStamp() +
                       " - SyncTables.PeriodicSync - Tabella :" + this.tableList[i] +
                       " inizio delete: " + CustomTimeStamp.GetTimeStamp());

                        if (enable) this.tableList[i].Synchronize(this.periodic, this.configData.maxRows, true);

                        log.Log(LogLevels.Info, CustomTimeStamp.GetTimeStamp() +
                        " - SyncTables.PeriodicSync - Tabella :" + this.tableList[i] +
                        " fine delete: " + CustomTimeStamp.GetTimeStamp());
                    }

                    catch (Exception Ex)
                    {
                        //log
                        log.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                           " - SyncTables.PeriodicSync (Delete) - " + Ex.Message);
                    }
                    
                }
               
                if (enable) Thread.Sleep(this.configData.intervalSync);
            }
        }

        private void OnceSync()
        {
            try
            {
                foreach (Table currentTable in this.tableList)
                    currentTable.Synchronize(this.periodic, this.configData.maxRows,false);

                for(int i= this.tableList.Count-1; i>=0; i--)
                    this.tableList[i].Synchronize(this.periodic, this.configData.maxRows, true);
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - SyncTables.OnceSync - " + Ex.Message);
            }
                
        }
        #endregion PrivateMethod

        #region Dispose
        private void DisposeTableList(List<Table> _tableList)
        {
            if (_tableList != null)
                foreach (Table t in _tableList)
                    t.Dispose();
                
        }

        private void DisposeDbList(List<Database> _db)
        {
            if (_db != null)
                foreach (Database d in _db)
                    d.Dispose();

        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.servers.Dispose();
            //this.configData.Dispose();
            this.DisposeDbList(this.DbList);
            this.DisposeTableList(this.tableList);
            if(log!=null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion Dispose

    }// END CLASS DEFINITION SyncTables
}