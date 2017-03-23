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
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{

    /// <summary>
    /// Contiene i parametri di configurazione
    /// </summary>
    public class ConfigurationInfo : IDisposable
    {
        #region PublicField

        /// <summary>
        /// Server locale
        /// </summary>
        public ServerInfo localServer;

        /// <summary>
        /// Server remoto
        /// </summary>
        public ServerInfo remoteServer;

        /// <summary>
        /// Periodo di esecuzione della sincronizzazione, 
        /// epsresso in secondi.
        /// </summary>
        public int intervalSync = 0;

        /// <summary>
        /// Periodo del controllo di connettività
        /// ai server, espresso in millisecondi.
        /// </summary>
        public int intervalConnectionController = 0;

        /// <summary>
        /// Nome della tabella che registra gli eventi di sincronizzazione.
        /// </summary>
        public string tableSyncName = string.Empty;

        /// <summary>
        /// Nome della colonna della tabella di sincronizzazione,
        /// che contiene il nome della tabella.
        /// </summary>
        public string tableSyncColumnTableName = string.Empty;

        /// <summary>
        /// Nome della colonna della tabella di sincronizzazione,
        /// che contiene l'istante in cui è avvenuta la sincronizzazione "Insert".
        /// </summary>
        public string tableSyncColumnDateTimeNameInsert = string.Empty;

        /// <summary>
        /// Nome della colonna della tabella di sincronizzazione,
        /// che contiene l'istante in cui è avvenuta la sincronizzazione "Update".
        /// </summary>
        public string tableSyncColumnDateTimeNameUpdate = string.Empty;


        /// <summary>
        /// Numero massimo di righe oggetto della sincronizzazione
        /// periodica.
        /// </summary>
        public uint maxRows = 0;

        /// <summary>
        /// Nome della extended property che indica se la tabella/database
        /// è oggetto del processo di sincronizzazione.
        /// </summary>
        public string extPropertyNameSync = string.Empty;

        /// <summary>
        /// Nome della extended property che indica il tipo di tabella
        /// </summary>
        public string extPropertyNameType = string.Empty;

        /// <summary>
        /// Nome della extended property che indica se riga è cancellata oppure no.
        /// </summary>
        public string extPropertyNameIsDeleted = string.Empty;

        /// <summary>
        /// Nome della extended property che contiene l'istante dell'ultimo
        /// aggiornamento.
        /// </summary>
        public string extPropertyLastUpdate = string.Empty;

        #endregion PublicField

        #region PrivateField

        //public RegistryKeys regKeys;

        private ParamsName parameters = new ParamsName();

        private IMessageLog log;

        private ParamTable paramTable;

        #endregion PrivateField

        #region Constructor

        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="_localServer">Server definito come locale</param>
        /// <param name="_remoteServer">Server definito come remoto</param>
        /// <param name="_paramTable">Tabella dei parametri</param>
        public ConfigurationInfo(ServerInfo _localServer, ServerInfo _remoteServer, ParamTable _paramTable)
        {
            try
            {
                log = LoggerToFile.LoggerToFile_singleton.GetLogger();
                this.localServer = _localServer;
                this.remoteServer = _remoteServer;
                this.paramTable = _paramTable;

                if (this.localServer != null) this.ReadParams(this.localServer);
                else if (this.remoteServer != null) this.ReadParams(this.remoteServer);

                if (_localServer != null) this.localServer.periodicTime = this.intervalConnectionController * 1000;
                if (_remoteServer != null) this.remoteServer.periodicTime = this.intervalConnectionController * 1000;
            }
            catch (Exception ex)
            {
                //log
                log.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                    " - ConfigurationInfo.ConfigurationInfo - " +
                    ex.Message);
            }
            finally
            {

            }
        }
        #endregion Constructor

        #region PrivateMethod
        private void ReadParams(ServerInfo _currentServer)
        {
            SqlCommand cmd = new SqlCommand();

            try
            {
                SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                conn.DataSource = _currentServer.localName;
                //conn.InitialCatalog = this.regKeys.GetParamDbName();
                conn.InitialCatalog = this.paramTable.DbName;
                conn.UserID = _currentServer.userName;
                conn.Password = _currentServer.password;
                conn.Pooling = false;
                
                cmd.Connection = new SqlConnection(conn.ConnectionString);
                cmd.CommandTimeout = _currentServer.commandTimeOut;

                cmd.Connection.Open();

                cmd.CommandText = this.GetCommand(ParamsName.Interval);
                this.intervalSync = int.Parse((string)cmd.ExecuteScalar());

                cmd.CommandText = this.GetCommand(ParamsName.IsDeleted);
                this.extPropertyNameIsDeleted = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.LastUpdate);
                this.extPropertyLastUpdate = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.MaxRows);
                this.maxRows = uint.Parse((string)cmd.ExecuteScalar());

                cmd.CommandText = this.GetCommand(ParamsName.Synchronize);
                this.extPropertyNameSync = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.TableSync);
                this.tableSyncName = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.TableName);
                this.tableSyncColumnTableName = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.Type);
                this.extPropertyNameType = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.DateTimeNameInsert);
                this.tableSyncColumnDateTimeNameInsert = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.DateTimeNameUpdate);
                this.tableSyncColumnDateTimeNameUpdate = (string)cmd.ExecuteScalar();

                cmd.CommandText = this.GetCommand(ParamsName.IntervalConnectionController);
                this.intervalConnectionController = int.Parse((string)cmd.ExecuteScalar());

                cmd.Connection.Close();
            }

            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                   " - ConfigurationInfo.ReadParams - " +
                   Ex.Message + "  Comando:" + cmd.CommandText);
            }

            finally
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Open) cmd.Connection.Close();
                cmd.Dispose();

            }

        }

        private string GetCommand(string _paramName)
        {
            return "SELECT VALUE FROM " + this.paramTable.DbName + "." + this.paramTable.Schema
                    + "." + this.paramTable.TableName + " WHERE NAME = '" + _paramName + "'";
        }

        #endregion PrivateMethod

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //if (this.localServer != null) this.localServer.Dispose();
            //if (this.remoteServer != null) this.remoteServer.Dispose();
            //this.regKeys.Dispose();
            if (log != null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion
    }
}// END CLASS DEFINITION ConfigurationInfo

