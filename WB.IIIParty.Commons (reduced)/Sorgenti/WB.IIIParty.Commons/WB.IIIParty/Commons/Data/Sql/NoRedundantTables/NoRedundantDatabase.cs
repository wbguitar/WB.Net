// ------------------------------------------------------------------------
//Societ?:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina  ll
//Nome modulo software: SyncTablesCommons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Threading;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;
using WB.IIIParty.Commons.Data.Sql.SyncTablesCommons;
namespace WB.IIIParty.Commons.Data.Sql.NoRedundantTables
{
    /// <summary>
    /// Oggetto database.
    /// </summary>
    public class NoRedundantDatabase
    {
        #region PublicField
        /// <summary>
        /// Server ove ? definito il database.
        /// </summary>
        public SingleServerInfo server;
        /// <summary>
        /// Nome del database
        /// </summary>
        public string name;
   

        #endregion PublicField

        #region PrivateField
        private IMessageLog log;
        #endregion PrivateField
        
        #region Contructor

        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="_servers"></param>
        /// <param name="_name"></param>
        public NoRedundantDatabase(SingleServerInfo _server, string _name)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.server = _server;
            this.name = _name;
           
        }

        #endregion Contructor

        #region PublicMethod
        ///// <summary>
        ///// Restituisce l'elenco delle viste.
        ///// </summary>
        /////// <param name="_propertyIsDeleted"></param>
        ///// <returns></returns>
        //public List<NoRedundantView> GetLocalViewList()//string _propertyIsDeleted)
        //{
        //    List<NoRedundantView> result = new List<NoRedundantView>();

        //    SqlCommand cmd = new SqlCommand();
        //    if (this.server.status == ConnectionStatus.Good)
        //    {
        //        try
        //        {
        //            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
        //            conn.DataSource = this.server.localName;
        //            conn.InitialCatalog = this.name;
        //            conn.UserID = this.server.userName;
        //            conn.Password = this.server.password;
        //            conn.Pooling = false;
                    
        //            cmd.CommandTimeout = this.server.commandTimeOut;
                    
        //            DataTable viewList = this.ExtractViewList(conn.ConnectionString, ref cmd);

                  

        //            foreach (DataRow current in viewList.Rows)
        //            {
                        
        //                result.Add(new NoRedundantView(this, (string)current.ItemArray[1], (string)current.ItemArray[0],
        //                            this.GetAllColumns(conn.ConnectionString, ref cmd, (string)current.ItemArray[0])));
                        
        //            }
        //        }

        //        catch (Exception Ex)
        //        {
        //            //log
        //            log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
        //               " - Database.GetLocalViewList - " + Ex.Message);
        //        }

        //        finally
        //        {
        //            if (cmd.Connection.State == System.Data.ConnectionState.Open)
        //                cmd.Connection.Close();
        //            cmd.Dispose();
        //        }
        //    }
        //    return result;
        //}




        /// <summary>
        /// Estrae l'elenco delle tabelle definite nel database.
        /// </summary>
        ///// <param name="_propertySyncName">Nome della extended property che indica se la tabella
        ///// ? soggetta a sincronizzazione oppure no.</param>
        ///// <param name="_syncValue">Valore della extended property '_propertySyncName'</param>
        ///// <param name="_propertyTypeName">Nome extended property che indica il tipo di tabella</param>
        ///// <param name="_typeValue">Valore della extended '_propertyTypeName'</param>
        ///// <param name="_propertyLastUpdate">Nome della extended property che indica il nome della colonna
        ///// che contiene dell'istante dell'ultimo aggiornamento della riga </param>
        ///// <param name="_propertyIsDeleted">Nome della extended property che indica il nome della colonna
        ///// che indica se la riga ? cancellata.</param>
        /// <returns>Elenco delle tabelle definite.</returns>
        //public List<NoRedundantTable> GetLocalTableList()
        //    //string _propertySyncName,
        //    //                         byte _syncValue,
        //    //                         string _propertyTypeName,
        //    //                         TableType _typeValue,
        //    //                         string _propertyLastUpdate,
        //    //                         string _propertyIsDeleted)
        //{
        //    List<NoRedundantTable> result = new List<NoRedundantTable>();

        //    SqlCommand cmd = new SqlCommand();
        //    if (this.server.status == ConnectionStatus.Good)
        //    {
        //        try
        //        {
        //            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
        //            conn.DataSource = this.server.localName;
        //            conn.InitialCatalog = this.name;
        //            conn.UserID = this.server.userName;
        //            conn.Password = this.server.password;
        //            //Modifica
        //            conn.Pooling = false;
        //            //
        //            cmd.CommandTimeout = this.server.commandTimeOut;

        //            DataTable tableList = this.ExtractTableList(conn.ConnectionString, ref cmd);//, null,0, null);

        //            string lastUpdate = string.Empty;
        //            string isDelete = string.Empty;

        //            foreach (DataRow current in tableList.Rows)
        //            {
        //                //if ((_propertyLastUpdate != null) && (_propertyLastUpdate != string.Empty))
        //                //    lastUpdate = this.GetValueExtProperty(conn.ConnectionString, ref cmd, _propertyLastUpdate, (string)current.ItemArray[1], (string)current.ItemArray[0], "TABLE");

        //                //if ((_propertyIsDeleted != null) && (_propertyIsDeleted != string.Empty))
        //                //    isDelete = this.GetValueExtProperty(conn.ConnectionString, ref cmd, _propertyIsDeleted, (string)current.ItemArray[1], (string)current.ItemArray[0], "TABLE");

        //                result.Add(new NoRedundantTable(this, (string)current.ItemArray[1], (string)current.ItemArray[0],//_typeValue,
        //                            this.GetPrimaryKey(conn.ConnectionString, ref cmd, (string)current.ItemArray[0]),
        //                            this.GetAllColumns(conn.ConnectionString, ref cmd, (string)current.ItemArray[0])));
        //                            //lastUpdate, isDelete Convert.ToInt32(current.ItemArray[2])));

        //            }
        //            tableList.Dispose();
        //        }

        //        catch (Exception Ex)
        //        {
        //            //log
        //            log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
        //               " - Database.GetLocalTableList - " + Ex.Message);
        //        }

        //        finally
        //        {
        //            if (cmd.Connection.State == System.Data.ConnectionState.Open)
        //                cmd.Connection.Close();
        //            cmd.Dispose();
        //        }
        //    }
        //    return result;
        //}

     #endregion PublicMethod
        
        #region PrivateMethod

        private DataTable ExtractViewList(string _conn, ref SqlCommand _cmd)
        {
            DataTable viewList = new DataTable();

            
            if ((_conn != null) && (_conn != string.Empty) &&
                (_cmd != null))
            {
                try
                {
                    _cmd.Connection = new SqlConnection(_conn);

                    _cmd.CommandText = "SELECT UPPER(V.NAME) AS 'ViewName'" +
                                    " , UPPER(S.NAME) AS 'SchemaName'" +
                                    " FROM SYS.VIEWS AS V, SYS.SCHEMAS AS S "+
                                    " WHERE V.SCHEMA_ID=S.SCHEMA_ID" +
                                    " and V.type_desc='VIEW'";
                   
                    _cmd.Connection.Open();

                    viewList.Columns.Add("ViewName");
                    viewList.Columns.Add("SchemaName");

                    object[] itemRead = new object[2];
                    SqlDataReader reader = _cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reader.GetValues(itemRead);
                        viewList.Rows.Add(itemRead);
                    }
                    reader.Close();
                    _cmd.Connection.Close();

                }
                catch (Exception Ex)
                {
                    //log
                    log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                       " - Database.ExtractViewList - " + Ex.Message);
                }

                finally
                {
                    if (_cmd.Connection.State == System.Data.ConnectionState.Open)
                        _cmd.Connection.Close();
                }
            }

            return viewList;
        }

        /// <summary>
        /// Legge dal database i nomi e gli schema,
        ///// delle tabelle con:
        ///// _propertySyncName = _syncValue e
        ///// _propertyTypeName = _typeValue
        ///// </summary>
        /// <param name="_conn"></param>
        /// <param name="_cmd"></param>
        ///// <param name="_propertySyncName"></param>
        ///// <param name="_syncValue"></param>
        ///// <param name="_propertyTypeName"></param>
        /////// <param name="_typeValue"></param>
        /// <returns></returns>
        private DataTable ExtractTableList(string _conn, ref SqlCommand _cmd)
                                     //string _propertySyncName,
                                     //byte _syncValue,
                                     //string _propertyTypeName)
            //,
              //                       TableType _typeValue)
        {
            DataTable tableList = new DataTable();
            if ((_conn != null) && (_conn != string.Empty) &&
                (_cmd != null))
            {
                try
                {
                    _cmd.Connection = new SqlConnection(_conn);
                    _cmd.CommandText = "SELECT UPPER(T.NAME) AS 'TableName', UPPER(S.NAME) AS 'SchemaName' FROM SYS.TABLES AS T, SYS.SCHEMAS AS S";
                                       
                    _cmd.Connection.Open();

                    tableList.Columns.Add("TableName");
                    tableList.Columns.Add("SchemaName");

                    object[] itemRead = new object[2];
                    SqlDataReader reader = _cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reader.GetValues(itemRead);
                        tableList.Rows.Add(itemRead);
                    }
                    reader.Close();
                    _cmd.Connection.Close();

                }
                catch (Exception Ex)
                {
                    
                    //log
                    log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                       " - Database.ExtractTableList - ( "+_cmd.Connection.Database+" ) " + Ex.Message);
                }

                finally
                {
                    if (_cmd.Connection.State == System.Data.ConnectionState.Open)
                        _cmd.Connection.Close();
                }
            }

            return tableList;
        }

        /// <summary>
        /// Estrae dal database il valore della propriet? estesa
        /// "_extPropertyName".
        /// </summary>
        /// <param name="_conn">Stringa di connessione al database </param>
        /// <param name="_cmd">Istruzione sql da eseguire</param>
        /// <param name="_extPropertyName">Nome della extended property da leggere</param>
        /// <param name="_schema">Schema di appartenenza della tabella o della vista</param>
        /// <param name="_table">Nome della tabella o della vista</param>
        /// <param name="_objType">Specifica il tipo di oggetto: "TABLE" o "VIEW"</param>
        /// <returns>Restituisce il valore della propriet? estesa "_extPropertyName"</returns>
        ///        
        private string GetValueExtProperty(string _conn, ref SqlCommand _cmd,
                                           string _extPropertyName, 
                                           string _schema, string _table, string _objType)
        {
            string result = string.Empty;
            if ((_conn != null) && (_conn != string.Empty) &&
                (_cmd != null) && (_extPropertyName != null) && (_extPropertyName != string.Empty) &&
                (_schema != null) && (_schema != string.Empty) &&
                (_table != null) && (_table != string.Empty)
                )
            {
                try
                {
                    
                    _cmd.Connection = new SqlConnection(_conn);
                    string command = " SELECT value as 'ExpPropValue'" +
                                     " FROM fn_listextendedproperty(" +
                                     "'" + _extPropertyName + "', 'SCHEMA'," +
                                     "'" + _schema + "', '"+_objType+"', " +
                                     "'" + _table + "' , null, null)";
                    _cmd.CommandText = command;
                    
                    _cmd.Connection.Open();
                    SqlDataReader reader = _cmd.ExecuteReader();

                    while (reader.Read())
                        result = (string)reader.GetValue(0);
                    reader.Close();
                    _cmd.Connection.Close();
                }

                catch (Exception Ex)
                {
                    //log
                    log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                       " - IdThread: " + Thread.CurrentThread.ManagedThreadId + " - " +
                       "Exception:Database - GetValueExtProperty - " + Ex.Message);
                }

                finally
                {
                    if (_cmd.Connection.State == System.Data.ConnectionState.Open)
                        _cmd.Connection.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Estrae l'elenco dei nomi delle colonne
        /// che compongono la chiave primaria della tabella.
        /// </summary>
        /// <param name="_conn"></param>
        /// <param name="_cmd"></param>
        /// <param name="_tableName"></param>
        /// <returns></returns>
        private List<string> GetPrimaryKey(string _conn, ref SqlCommand _cmd, string _tableName)
        {
            List<string> result = new List<string>();
            if ((_conn != null) && (_conn != string.Empty) &&
                (_cmd != null) && (_tableName != null) && (_tableName != string.Empty)
                )
            {
                try
                {
                    _cmd.Connection = new SqlConnection(_conn);
                    string command = " EXEC SP_PKEYS @TABLE_NAME = N'" + _tableName +
                                       "' ,  @TABLE_QUALIFIER ='" + this.name + "'";
                    _cmd.CommandText = command;
                    _cmd.Connection.Open();

                    SqlDataReader reader = _cmd.ExecuteReader();

                    while (reader.Read())
                        result.Add(((string)reader.GetValue(3)).ToUpper());

                    reader.Close();
                    _cmd.Connection.Close();
                }

                catch (Exception Ex)
                {
                    //log
                    log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                       " - IdThread: " + Thread.CurrentThread.ManagedThreadId + " - " +
                       "Exception: Database - GetPrimaryKey - " + Ex.Message);

                }

                finally
                {
                    if (_cmd.Connection.State == System.Data.ConnectionState.Open)
                        _cmd.Connection.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// Estrae l'elenco dei nomi delle colonne
        /// che compongono la chiave primaria della tabella.
        /// </summary>
        /// <param name="_conn"></param>
        /// <param name="_cmd"></param>
        /// <param name="_tableName"></param>
        /// <returns></returns>
        private List<string> GetAllColumns(string _conn, ref SqlCommand _cmd, string _tableName)
        {
            List<string> result = new List<string>();
            if ((_conn != null) && (_conn != string.Empty) &&
                (_cmd != null) && (_tableName != null) && (_tableName != string.Empty)
                )
            {
                try
                {
                    _cmd.Connection = new SqlConnection(_conn);
                    string command = " EXEC SP_COLUMNS @TABLE_NAME = N'" + _tableName +
                                       "' ,  @TABLE_QUALIFIER ='" + this.name + "'";
                    _cmd.CommandText = command;
                    _cmd.Connection.Open();

                    SqlDataReader reader = _cmd.ExecuteReader();

                    while (reader.Read())
                        result.Add(((string)reader.GetValue(3)).ToUpper());

                    reader.Dispose();
                    _cmd.Connection.Close();
                }

                catch (Exception Ex)
                {
                    //log
                    log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                       " - IdThread: " + Thread.CurrentThread.ManagedThreadId + " - " +
                       "Exception: Database - GetPrimaryKey - " + Ex.Message);

                }

                finally
                {
                    if (_cmd.Connection.State == System.Data.ConnectionState.Open)
                        _cmd.Connection.Close();
                }
            }

            return result;
        }

        #endregion PrivateMethod

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

    }// END CLASS DEFINITION Database
}

