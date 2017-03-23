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
using System.Collections;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{

    /// <summary>
    /// Reppresenta la coppia di database oggetto
    /// del processo di sincronizzazione.
    /// </summary>
    public class SyncServers: IDisposable
    {
        #region PrivateField
        private IMessageLog log;
        #endregion PrivateField

        #region PublicField
        /// <summary>
        /// Server definito come locale
        /// </summary>
        public ServerInfo localServer ;
        /// <summary>
        /// Server definito come remoto
        /// </summary>
        public ServerInfo remoteServer;

        #endregion PublicField
        
        #region Constructor
        /// <summary>
        /// Coppia di server oggetto del processo di sincronizzazione.
        /// </summary>
        /// <param name="_localServer">Server definito come locale</param>
        /// <param name="_remoteServer">Server definito come remoto</param>
        public  SyncServers(ServerInfo _localServer, ServerInfo _remoteServer)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.localServer = _localServer;
            this.remoteServer = _remoteServer;
        }

        #endregion Constructor

        #region PublicMethod
        /// <summary>
        /// Estrae la lista dei database appartenenti ad un server
        /// con extended property pari a "_propertySyncName" il 
        /// cui valore è "_syncValue"
        /// </summary>
        /// <param name="_propertySyncName">Nome extended property.</param>
        /// <param name="_syncValue">Valore della extended property.</param>
        /// <returns></returns>
        public List<Database> GetLocalDbList(string _propertySyncName,
                                     byte _syncValue)
        {
            List<Database> result = new List<Database>();         
            SqlCommand cmd = new SqlCommand();
            ArrayList databases = new ArrayList();
            try
            {
                SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                conn.DataSource = this.localServer.localName;
                conn.InitialCatalog = "master";
                conn.UserID = this.localServer.userName;
                conn.Password = this.localServer.password;
                conn.Pooling = false;
                
                cmd.Connection = new SqlConnection(conn.ConnectionString);
                cmd.CommandTimeout = this.localServer.commandTimeOut;
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
                                result.Add(new Database(this, current.ToString()));
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
                            result.Add(new Database(this, current.ToString()));
                    }
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
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

        #region Dispose
        /// <summary>
        /// Dispose dell'oggetto.
        /// </summary>
        public void Dispose()
        {
            if (log != null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion Dispose

    }
}


