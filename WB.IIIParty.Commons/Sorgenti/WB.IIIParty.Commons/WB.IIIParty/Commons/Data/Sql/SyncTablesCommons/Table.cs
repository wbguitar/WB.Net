// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina
//Nome modulo software: SyncTablesCommons.dll
//Data ultima modifica: $LastChangedDate: 2012-12-18 16:54:43 +0100 (mar, 18 dic 2012) $
//Versione:             $Rev: 117 $
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
    /// Rappresenta una tabella di database.
    /// </summary>
    public class Table: IDisposable
    {
        #region Static
        private static string CustomDateTime = "yyyyMMdd HH:mm:ss.fff";
        private static DateTime defaultSyncDate = new DateTime(2000,01,01,00,00,00);
        #endregion Static

        #region PublicProperty
        /// <summary>
        /// Nome della tabella
        /// </summary>
        public string Name  
        {
            get { return this.name.ToUpper(); }
            set 
            { 
                this.name = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
                if ((this.completeName != null) && (this.completeName != ""))
                    this.completeName = this.completeName.ToUpper();
            }
        }

        /// <summary>
        /// Schema al quale la tabella appartiene
        /// </summary>
        public string Schema
        {
            get { return this.schema.ToUpper(); }
            set
            {
                this.schema = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
                if ((this.completeName != null) && (this.completeName != "")) 
                    this.completeName = this.completeName.ToUpper();
            }
        }

        /// <summary>
        /// Nome del database al quale la tabella appartiene.
        /// </summary>
        public Database Db
        {
            get { return this.db; }
            set
            {
                this.db = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
                if ((this.completeName != null) && (this.completeName != ""))
                    this.completeName = this.completeName.ToUpper();
            }
        }

        /// <summary>
        /// Tipo della tabella 
        /// </summary>
        public TableType Type
            {
                set { this.type = value; }
                get { return this.type; }
            }

        /// <summary>
        /// Nome completo della tabella: 
        /// "DbName.SchemaName.TableName".
        /// </summary>
        public string CompleteName
        {
            get { return this.completeName.ToUpper(); }
        }

        /// <summary>
        /// Livello di relazione fra tabelle in un db
        /// </summary>
        public Int32 Level
        {
             get { return this.level; }
        }

        #endregion PublicProperty

        #region PrivateField
        
        private Int32 level = 1;
        private string completeName = string.Empty;
        private string name;
        private string schema;
        private Database db;
        private IMessageLog log;
        private TableType type;

        #endregion PrivateField
                
        #region PublicField

        /// <summary>
        /// Elenco delle colonne che compongono la chiave primaria.
        /// </summary>
        public List<string> primaryKeys;

        /// <summary>
        /// Elenco di tutte le colonne.
        /// </summary>
        public List<string> allColumns;

        /// <summary>
        /// Nome della colonna che contiene la data 
        /// dell'ultimo aggiornamento della riga.
        /// </summary>
        public string lastUpdateColumnName;

        /// <summary>
        /// Nome della colonna che indica se la riga è
        /// cancellata.
        /// </summary>
        public string isDeletedColumnName;

        /// <summary>
        /// Numero massimo di righe oggetto della 
        /// sincronizzazione.
        /// </summary>
        public uint maxRowToSync = 0;

        
        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_db">Database di appartenenza</param>
        /// <param name="_schema">Schema di appartenenza</param>
        /// <param name="_name">Nome della tabella</param>
        /// <param name="_type">Tipo</param>
        /// <param name="_primaryKeys">Elenco delle colonne che costituiscono la chiave primaria</param>
        /// <param name="_allColumns">Elenco di tutte le colonne</param>
        /// <param name="_lastUpdateColumnName">Nome della colonna che indica l'istante dell'ultimo 
        /// aggiornamento</param>
        /// <param name="_isDeletedColumnName">Nome della colonna che indica se la riga è cancellata</param>
        public Table(Database _db,
                       string _schema,
                       string _name,
                       TableType _type,
                       List<string> _primaryKeys,
                       List<string> _allColumns,
                       string _lastUpdateColumnName,
                       string _isDeletedColumnName)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.Db = _db;
            this.Schema = _schema;
            this.Name = _name;
            this.primaryKeys = _primaryKeys;
            this.allColumns = _allColumns;
            this.lastUpdateColumnName = _lastUpdateColumnName.ToUpper();
            this.isDeletedColumnName = _isDeletedColumnName.ToUpper();
            this.Type = _type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_db"></param>
        /// <param name="_schema"></param>
        /// <param name="_name"></param>
        /// <param name="_type"></param>
        /// <param name="_primaryKeys"></param>
        /// <param name="_allColumns"></param>
        /// <param name="_lastUpdateColumnName"></param>
        /// <param name="_isDeletedColumnName"></param>
        /// <param name="_level"></param>
        public Table(Database _db,
                       string _schema,
                       string _name,
                       TableType _type,
                       List<string> _primaryKeys,
                       List<string> _allColumns,
                       string _lastUpdateColumnName,
                       string _isDeletedColumnName,
                       Int32 _level)
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.Db = _db;
            this.Schema = _schema;
            this.Name = _name;
            this.primaryKeys = _primaryKeys;
            this.allColumns = _allColumns;
            this.lastUpdateColumnName = _lastUpdateColumnName.ToUpper();
            this.isDeletedColumnName = _isDeletedColumnName.ToUpper();
            this.Type = _type;
            this.level = _level;
        }


        #endregion Constructor
        
        #region PublicMethod
        

        /// <summary>
        /// Sincronizza le rgihe delle due tabelle presenti nelle
        /// due istanze server.
        /// </summary>
        /// <param name="_periodic">Indica se il processo è periodico oppure no.</param>
        /// <param name="_maxRows">Numero massimo di righe oggetto di ogni processo di sincronizzazione.</param>
        /// <param name="_onlyDelete">True nel caso di sola conacellazione delle righe con isDeleted=1,
        /// False altrimenti</param>
        public void Synchronize(bool _periodic, uint _maxRows, bool _onlyDelete)
        {
            DateTime current = DateTime.Now.ToUniversalTime();
            object[] currentMaxTime = new object[4];
            int indice = 0;

            if (_periodic) this.maxRowToSync = _maxRows;
            else this.maxRowToSync = uint.MaxValue;

            if ((this.db.servers.localServer.status == ConnectionStatus.Good) &&
                (this.db.servers.remoteServer.status == ConnectionStatus.Good))
            {
                SqlCommand cmd = new SqlCommand();
                SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
                connection.DataSource = this.db.servers.localServer.localName;
                connection.InitialCatalog = this.db.name;
                connection.Password = this.db.servers.localServer.password;
                connection.UserID = this.db.servers.localServer.userName;
                connection.Pooling = false;
                
                cmd.Connection = new SqlConnection();
                cmd.Connection.ConnectionString = connection.ConnectionString;
                cmd.CommandTimeout = this.db.servers.localServer.commandTimeOut;

                //Estraggo la data ed ora dell'ultimo processo di sincronizzazione
                //con esito positivo su entrambe le tabelle
                System.Nullable<DateTime> lastSyncTimeInsert = new DateTime();
                System.Nullable<DateTime> lastSyncTimeUpdate = new DateTime();
                System.Nullable<DateTime> localLastSyncTimeInsert = this.GetLastSyncTime(cmd, this.db.servers.localServer.localName, true);
                System.Nullable<DateTime> localLastSyncTimeUpdate = this.GetLastSyncTime(cmd, this.db.servers.localServer.localName,false);
                //System.Nullable<DateTime> remoteLastSyncTimeInsert = this.GetLastSyncTime(cmd, this.db.servers.localServer.localLinkedName, true);
                //System.Nullable<DateTime> remoteLastSyncTimeUpdate = this.GetLastSyncTime(cmd, this.db.servers.localServer.localLinkedName, false);
                System.Nullable<DateTime> remoteLastSyncTimeInsert = this.GetLastSyncTime(cmd, this.db.servers.remoteServer.localName, true);
                System.Nullable<DateTime> remoteLastSyncTimeUpdate = this.GetLastSyncTime(cmd, this.db.servers.remoteServer.localName, false);
                if (localLastSyncTimeInsert < remoteLastSyncTimeInsert) lastSyncTimeInsert = localLastSyncTimeInsert;
                else lastSyncTimeInsert = remoteLastSyncTimeInsert;

                if (localLastSyncTimeUpdate < remoteLastSyncTimeUpdate) lastSyncTimeUpdate = localLastSyncTimeUpdate;
                else lastSyncTimeUpdate = remoteLastSyncTimeUpdate;

                if (!_onlyDelete)
                {

                    //Estraggo la data massima delle righe oggetto della insert da locale in remoto
                    int RowsAffected = 0;
                    //currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.localServer.localName,
                    //                                 this.db.servers.localServer.localLinkedName, lastSyncTimeInsert, true);

                    currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.localServer.localName,
                                                     this.db.servers.remoteServer.localName, lastSyncTimeInsert, true);

                    if (currentMaxTime[indice] != null)
                        //RowsAffected = this.SincrInsert(cmd, this.db.servers.localServer.localName,
                        //               this.db.servers.localServer.localLinkedName, lastSyncTimeInsert,
                        //               (System.Nullable<DateTime>)currentMaxTime[indice]);
                         RowsAffected = this.SincrInsert(cmd, this.db.servers.localServer.localName,
                                       this.db.servers.remoteServer.localName, lastSyncTimeInsert,
                                       (System.Nullable<DateTime>)currentMaxTime[indice]);
                    else currentMaxTime[indice] = current;

                    indice++;

                    //Estrazione della data massima delle righe oggetto della insert da remoto in locale
                    //currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.localServer.localLinkedName,
                    //                                 this.db.servers.localServer.localName, lastSyncTimeInsert, true);

                    currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.remoteServer.localName,
                                                     this.db.servers.localServer.localName, lastSyncTimeInsert, true);
                    if (currentMaxTime[indice] != null)
                        //RowsAffected = this.SincrInsert(cmd, this.db.servers.localServer.localLinkedName,
                        //               this.db.servers.localServer.localName, lastSyncTimeInsert,
                        //               (System.Nullable<DateTime>)currentMaxTime[indice]);
                        RowsAffected = this.SincrInsert(cmd, this.db.servers.remoteServer.localName,
                                       this.db.servers.localServer.localName, lastSyncTimeInsert,
                                       (System.Nullable<DateTime>)currentMaxTime[indice]);
                    else currentMaxTime[indice] = current;

                    indice++;

                    //Update righe da locale in remoto
                    //currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.localServer.localName,
                    //                                this.db.servers.localServer.localLinkedName, lastSyncTimeUpdate, false);

                    currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.localServer.localName,
                                               this.db.servers.remoteServer.localName, lastSyncTimeUpdate, false);

                    if (currentMaxTime[indice] != null)
                        //RowsAffected = this.SincrUpdate(cmd, this.db.servers.localServer.localName,
                        //               this.db.servers.localServer.localLinkedName, lastSyncTimeUpdate,
                        //               (System.Nullable<DateTime>)currentMaxTime[indice]);
                          RowsAffected = this.SincrUpdate(cmd, this.db.servers.localServer.localName,
                                       this.db.servers.remoteServer.localName, lastSyncTimeUpdate,
                                       (System.Nullable<DateTime>)currentMaxTime[indice]);
                    else currentMaxTime[indice] = current;

                    indice++;

                    //Update righe da remoto in locale
                    //currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.localServer.localLinkedName,
                    //                               this.db.servers.localServer.localName, lastSyncTimeUpdate, false);

                    currentMaxTime[indice] = (object)this.GetCurrentMaxTime(cmd, this.db.servers.remoteServer.localName,
                                                   this.db.servers.localServer.localName, lastSyncTimeUpdate, false);

                    if (currentMaxTime[indice] != null)
                        //RowsAffected = this.SincrUpdate(cmd, this.db.servers.localServer.localLinkedName,
                        //               this.db.servers.localServer.localName, lastSyncTimeUpdate,
                        //               (System.Nullable<DateTime>)currentMaxTime[indice]);
                        RowsAffected = this.SincrUpdate(cmd, this.db.servers.remoteServer.localName,
                                       this.db.servers.localServer.localName, lastSyncTimeUpdate,
                                       (System.Nullable<DateTime>)currentMaxTime[indice]);
                    else currentMaxTime[indice] = current;


                    //Selezione dell'istante dell'ultima sincronizzazione
                    DateTime syncTimeInsert = defaultSyncDate.AddYears(1000);
                    DateTime syncTimeUpdate = defaultSyncDate.AddYears(1000);

                    if ((DateTime)currentMaxTime[0] < (DateTime)currentMaxTime[1]) syncTimeInsert = (DateTime)currentMaxTime[0];
                    else syncTimeInsert = (DateTime)currentMaxTime[1];

                    if ((DateTime)currentMaxTime[2] < (DateTime)currentMaxTime[3]) syncTimeUpdate = (DateTime)currentMaxTime[2];
                    else syncTimeUpdate = (DateTime)currentMaxTime[3];
                    

                    //aggiornmento data di sincronizzazione in TableSync
                    this.SetDateMax(cmd, this.db.servers.localServer.localName, syncTimeInsert, syncTimeUpdate);
                    //this.SetDateMax(cmd, this.db.servers.localServer.localLinkedName, syncTimeInsert, syncTimeUpdate);
                    this.SetDateMax(cmd, this.db.servers.remoteServer.localName, syncTimeInsert, syncTimeUpdate);

                }
                else
                {
                    this.DelDeletedRows(cmd, this.db.servers.localServer.localName, lastSyncTimeInsert);
                    //this.DelDeletedRows(cmd, this.db.servers.localServer.localLinkedName,  lastSyncTimeInsert);
                    this.DelDeletedRows(cmd, this.db.servers.remoteServer.localName, lastSyncTimeInsert);
                    this.DelDeletedRows(cmd, this.db.servers.localServer.localName, lastSyncTimeUpdate);
                    //this.DelDeletedRows(cmd, this.db.servers.localServer.localLinkedName, lastSyncTimeUpdate);
                    this.DelDeletedRows(cmd, this.db.servers.remoteServer.localName, lastSyncTimeUpdate);
                }
                if (cmd.Connection.State == System.Data.ConnectionState.Open) cmd.Connection.Close();
                cmd.Dispose();
            }
        }
        
        #endregion PublicMethod
        /// <summary>
        /// Timeout meggiorato per le operazioni di update 
        /// che sono più lunghe
        /// </summary>
        private int commandTimeout = 150;
        #region PrivateMethod
        /// <summary>
        /// Aggiorna le righe, del server di destinazione, con data inferiore 
        /// rispetto a quelle del server sorgente.
        /// </summary>
        /// <param name="_cmd">Connessione al db.</param>
        /// <param name="_serverSrc">Server sorgente</param>
        /// <param name="_serverDst">Server destinazione</param>
        /// <param name="_lastSyncTime">Istante dell'ultima sincronizzazione</param>
        /// <param name="_maxTime">Limite superiore dell'intervallo di tempo da considerare</param>
        /// <returns>Numero di righe aggiornate.</returns>
        private int SincrUpdate(SqlCommand _cmd, string _serverSrc, string _serverDst,  
                                System.Nullable<DateTime> _lastSyncTime, 
                                System.Nullable<DateTime> _maxTime)
        {
            int CurrentRowsAffected = 0;
            string aliasSrc = "Src";
            string aliasDst = "Dst";
            string conditionPk = string.Empty;
            string conditionTs = string.Empty;
            string setCommand = string.Empty;

            try
            {
                for (int i = 0; i < this.allColumns.Count; i++)
                {
                    if (!(this.primaryKeys.Contains(this.allColumns[i])))
                        if (setCommand == string.Empty) setCommand = aliasDst + "." + allColumns[i] + "=" + aliasSrc + "." + allColumns[i];
                        else setCommand = setCommand + " , " + aliasDst + "." + allColumns[i] + "=" + aliasSrc + "." + allColumns[i];
                }

                if (primaryKeys.Count>0)
                {
                    for (int i = 0; i < this.primaryKeys.Count; i++)
                    {
                        if (conditionPk == string.Empty)
                            conditionPk = aliasSrc + "." + this.primaryKeys[i] + "=" + aliasDst + "." + this.primaryKeys[i];
                        else conditionPk = conditionPk + " AND " + aliasSrc +"."+ this.primaryKeys[i] +
                                      "=" + aliasDst + "." + this.primaryKeys[i] ;
                    }
                }

                if (_lastSyncTime.HasValue)
                    conditionTs = aliasSrc + "." + this.lastUpdateColumnName + " >='" + ((DateTime)(_lastSyncTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'" +
                        " AND " + aliasSrc + "." + this.lastUpdateColumnName + " >" + aliasDst + "." + this.lastUpdateColumnName  +
                        " AND " + aliasSrc + "." + this.lastUpdateColumnName + " <='" + ((DateTime)(_maxTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";

                //Aggiorno le righe sul corrente db standby che hanno una data inferiore rispetto a quella presente nel corrente db master
                _cmd.CommandText = "UPDATE " + aliasDst + " SET " + setCommand + " from [" +
                    _serverSrc + "]." + this.completeName + " AS " + aliasSrc +
                    " , [" + _serverDst + "]." + this.completeName + " AS " + aliasDst +
                    " WHERE " + conditionPk; 
                if (conditionTs != string.Empty)
                    _cmd.CommandText = _cmd.CommandText + " AND " + conditionTs;
                
                _cmd.Connection.Open();
                _cmd.CommandTimeout = commandTimeout;
                CurrentRowsAffected = _cmd.ExecuteNonQuery();
                _cmd.Connection.Close();

                //Debug
                //log.WriteMessageToFile("Info: SincrUpdate - CurrentRowsAffected = " + CurrentRowsAffected.ToString() +
                //    "  da " + _serverSrc + "  a  " + _serverDst + "  _maxTime = " + ((DateTime)(_maxTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture)); 
            }

            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - Table.SincrUpdate - " + Ex.Message); 
            }
            finally
            {
                if (_cmd.Connection.State == System.Data.ConnectionState.Open) _cmd.Connection.Close();
                
            }

            if (CurrentRowsAffected < 0) CurrentRowsAffected = 0;

            return CurrentRowsAffected;
        }

        /// <summary>
        /// Aggiorna la tabella che registra gli eventi di sincronizzazione
        /// </summary>
        /// <param name="_cmd">Comando per la connessione </param>
        /// <param name="_server">Nome del server su cui fare l'aggiornamento</param>
        /// <param name="_syncTimeStampInsert">Istante dell'ultima sincronizzazione per insert</param>
        /// <param name="_syncTimeStampUpdate">Istante dell'ultima sincronizzazione per update</param>
        private void SetDateMax(SqlCommand _cmd, string _server, System.Nullable<DateTime> _syncTimeStampInsert, System.Nullable<DateTime> _syncTimeStampUpdate)
        {
            try
            {
                string timeInsert = string.Empty;
                string timeUpdate = string.Empty;

                if ((_syncTimeStampInsert.HasValue) || (_syncTimeStampUpdate.HasValue))
                {
                    _cmd.CommandText = "UPDATE [" + _server + "]." +
                                     this.db.name + "." + this.schema + "." + this.db.SyncTable.name + " SET ";

                    if (_syncTimeStampInsert.HasValue) timeInsert = this.db.SyncTable.colDateTimeInsert +
                                                        " ='" + ((DateTime)_syncTimeStampInsert).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";

                    if (_syncTimeStampUpdate.HasValue) timeUpdate = this.db.SyncTable.colDateTimeUpdate +
                                                        " ='" + ((DateTime)_syncTimeStampUpdate).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";

                    if (timeInsert != string.Empty) _cmd.CommandText = _cmd.CommandText + timeInsert + " , " + timeUpdate;
                    else _cmd.CommandText = _cmd.CommandText + timeUpdate;

                    _cmd.CommandText = _cmd.CommandText + " WHERE " + this.db.SyncTable.colTable + "='" + this.name + "'";

                    _cmd.Connection.Open();
                    _cmd.ExecuteNonQuery();
                    _cmd.Connection.Close();
                }
            }

            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - Table.SetDateMax - " + Ex.Message);    
            }

            finally
            {
                if (_cmd.Connection.State == System.Data.ConnectionState.Open) _cmd.Connection.Close();  
            }
        }


        /// <summary>
        /// Inserisce nella tabella Standby le righe presenti in serverSrc e non presenti 
        /// in serverDst
        /// </summary>
        /// <param name="_cmd">Comando per la connessione al server </param>
        /// <param name="_serverSrc">Server sorgente da cui estrarre le righe</param>
        /// <param name="_serverDst">Server destinazione in cui copiare le righe</param>
        /// <param name="_lastSyncTime">Istante in cui è avvenuta l'ultima sincronizzazione</param>
        /// <param name="_maxTime">Limite superiore per l'ultimo aggiornamento delle righe</param>
        /// <returns>Restituisce il numero di righe inserite.</returns>
        private int SincrInsert(SqlCommand _cmd, string _serverSrc, string _serverDst,
                                System.Nullable<DateTime> _lastSyncTime, System.Nullable<DateTime> _maxTime)
        {
            int CurrentRowsAffected = 0;
            string aliasSrc = "Src";
            string aliasDst = "Dst";
            string conditionPk = string.Empty;
            string conditionTs = string.Empty;

            try
            {
                //Inserisco le prime "maxRowToSync" righe non sincornizzate, nella tabella remota
                _cmd.CommandText = "INSERT INTO [" + _serverDst + "]."
                    + this.completeName + " SELECT TOP " + this.maxRowToSync + " * "
                    + " FROM [" + _serverSrc + "]."
                    + this.completeName + " AS " + aliasSrc + " WHERE "
                    + aliasSrc + "." + this.primaryKeys[0] + " NOT IN (SELECT "
                    + aliasDst + "." + this.primaryKeys[0]
                    + " FROM [" + _serverDst + "]."+ this.completeName+ " AS " + aliasDst;
                                     
                if (primaryKeys.Count>1)
                {
                    for (int i = 0; i < this.primaryKeys.Count; i++)
                    {
                        if (conditionPk == string.Empty)
                            conditionPk = aliasSrc + "." + this.primaryKeys[i] + "=" + aliasDst + "." + this.primaryKeys[i];
                        else conditionPk = conditionPk + " AND " + aliasSrc +"."+ this.primaryKeys[i] +
                                      "=" + aliasDst + "." + this.primaryKeys[i];
                    }
                }
                if (conditionPk != string.Empty)
                    _cmd.CommandText = _cmd.CommandText + " WHERE " + conditionPk ;
                _cmd.CommandText = _cmd.CommandText + ")";

                if (_lastSyncTime.HasValue)
                    //conditionTs = aliasDst + "." + this.lastUpdateColumnName + " >='" + ((DateTime)(_lastSyncTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "') AND " +
                    conditionTs = aliasSrc + "." + this.lastUpdateColumnName + " >='" + ((DateTime)(_lastSyncTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'" +
                        " AND " + aliasSrc + "." + this.lastUpdateColumnName + " <='" + ((DateTime)(_maxTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";

                if (conditionTs != string.Empty) _cmd.CommandText = _cmd.CommandText + " AND " + conditionTs;
                //else _cmd.CommandText = _cmd.CommandText + " )";
                _cmd.CommandText = _cmd.CommandText + " ORDER BY " + this.lastUpdateColumnName+" ASC ";

                _cmd.Connection.Open();
                CurrentRowsAffected = _cmd.ExecuteNonQuery();
                _cmd.Connection.Close();

                //Debug
                //log.WriteMessageToFile("SincrInsert - CurrentRowsAffected = "+CurrentRowsAffected.ToString()+
                //    "  da " + _serverSrc + "  a  " + _serverDst + "  _maxTime = " + ((DateTime)(_maxTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture)+" comando = " +_cmd.CommandText); 
                   
            }

            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - Table.SincrInsert - " + Ex.Message+" comando = " +_cmd.CommandText);    
            }
            finally
            {
                if (_cmd.Connection.State == System.Data.ConnectionState.Open) _cmd.Connection.Close();
            }
            if (CurrentRowsAffected < 0) CurrentRowsAffected = 0;

            return CurrentRowsAffected;
        }

        
        /// <summary>
        /// Estrae la data della precedente sincronizzazione 
        /// </summary>
        /// <returns>DateTime.</returns>
        private System.Nullable<DateTime> GetLastSyncTime(SqlCommand _cmd, string _server, bool _isInsert)
        {
            System.Nullable<DateTime> lastUpdateTime = null;
            
            try
            {
                string colDateTime = string.Empty;
                if (_isInsert) colDateTime = this.db.SyncTable.colDateTimeInsert;
                else colDateTime = this.db.SyncTable.colDateTimeUpdate;

                _cmd.CommandText = "SELECT [" + colDateTime + "] FROM " + 
                                    "["+_server +"]."+this.db.name + "." + 
                                    this.schema+"."+  this.db.SyncTable.name+
                                    " WHERE "+ this.db.SyncTable.colTable+"= '" +
                                    this.name + "'";
                _cmd.Connection.Open();
                object obj = _cmd.ExecuteScalar();
                    
                if (obj == DBNull.Value) lastUpdateTime = defaultSyncDate;
                else lastUpdateTime = (System.Nullable<DateTime>)obj;
                
                _cmd.Connection.Close();

                //Debug
                //if (lastUpdateTime != null)
                //    log.WriteMessageToFile("Debug: GetLastSyncTime - GetLastSyncTime = " + ((DateTime)(lastUpdateTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture));
                //else log.WriteMessageToFile("Debug: GetLastSyncTime - GetLastSyncTime = NULL "); 
                  
            }
            catch (Exception Ex)
            {
                lastUpdateTime = null;
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - Table.GetLastSyncTime - " + Ex.Message);
            }
                
            finally
            {
                if (_cmd.Connection.State == System.Data.ConnectionState.Open) _cmd.Connection.Close();
            }
            return lastUpdateTime;
            
        }
        
        //Estrae il valore massimo della data delle righe da sincronizzare
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd">Comando per la connessione al server</param>
        /// <param name="_server1">nome server sorgente</param>
        /// <param name="_server2">nome server destinazione</param>
        /// <param name="_lastSyncTime">istante dell'ultima sincronizzazione</param>
        /// <param name="_isInsert">Indica se l'attuale processo di sincronizzazione è
        /// per insert o per update</param>
        /// <returns>Data massima dell'intervallo di righe da sincronizzare</returns>
        private System.Nullable<DateTime> GetCurrentMaxTime(SqlCommand _cmd, string _server1, string _server2, System.Nullable<DateTime> _lastSyncTime, bool _isInsert)
        {
            System.Nullable<DateTime> MaxTime = null;
            
            try
            {
                string aliasSrc = "Src";
                string aliasDst = "Dst";
                string conditionPk = string.Empty;
                string conditionTs = string.Empty;
                string existRows = string.Empty;

                if (_isInsert) existRows = " NOT ";
                _cmd.CommandText = " SELECT MAX(" + this.lastUpdateColumnName
                                    + ") FROM ( SELECT TOP "
                                    + this.maxRowToSync + " " + this.lastUpdateColumnName
                                    + " FROM [" + _server1 + "]." + this.completeName + " AS " + aliasSrc + " WHERE "
                                    + aliasSrc + "." + this.primaryKeys[0] + existRows + " IN (SELECT "
                                    + aliasDst + "." + primaryKeys[0]
                                    + " FROM [" + _server2 + "]."
                                    + this.completeName + " AS " + aliasDst;
               
                for (int i = 0; i < this.primaryKeys.Count; i++)
                {
                    if (conditionPk == string.Empty) 
                        conditionPk = aliasSrc+"."+ this.primaryKeys[i] +"=" + aliasDst +"."+ this.primaryKeys[i];
                    else conditionPk = conditionPk + " AND " + aliasSrc +"."+ this.primaryKeys[i] +
                        "=" + aliasDst + "."+this.primaryKeys[i] ;
                }
                    
                

                if ((_lastSyncTime.HasValue)&&(_isInsert))
                    conditionTs = aliasSrc + "." + this.lastUpdateColumnName + " >='" + ((DateTime)(_lastSyncTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";

                if ((_lastSyncTime.HasValue) && !(_isInsert))
                    conditionTs = aliasSrc + "." + this.lastUpdateColumnName + ">" + aliasDst + "." + this.lastUpdateColumnName + ")" +
                                  " AND " + aliasSrc + "." + this.lastUpdateColumnName + " >='" + ((DateTime)(_lastSyncTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";

                if ((_isInsert) ||
                    ((!_lastSyncTime.HasValue) && (!_isInsert))
                    )
                    conditionPk = conditionPk + " ) ";

                if ((conditionPk != string.Empty) || (conditionTs != string.Empty))
                {
                    if ((conditionPk != string.Empty) && (conditionTs == string.Empty))
                        _cmd.CommandText = _cmd.CommandText + " WHERE " + conditionPk;

                    if ((conditionPk != string.Empty) && (conditionTs != string.Empty))
                        _cmd.CommandText = _cmd.CommandText + " WHERE " + conditionPk + " AND " + conditionTs;

                    if ((conditionPk == string.Empty) && (conditionTs != string.Empty))
                        _cmd.CommandText = _cmd.CommandText + " WHERE " + conditionTs;

                }
                //else _cmd.CommandText = _cmd.CommandText + " ) ";
                _cmd.CommandText = _cmd.CommandText + " ORDER BY " + this.lastUpdateColumnName+ " ASC ) AS TAB";
                _cmd.Connection.Open();
                object obj = _cmd.ExecuteScalar();
                if (obj == DBNull.Value) MaxTime = null;
                else MaxTime = (System.Nullable<DateTime>)obj;

                _cmd.Connection.Close();


                //Debug
                //if (MaxTime != null)
                //    log.WriteMessageToFile("Debug: GetCurrentMaxTime - MaxTime = " + ((DateTime)(MaxTime)).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "  da " + _server1 + "  a  " + _server2 + " Insert= "+_isInsert.ToString()+ " comando = " +_cmd.CommandText);
                //else log.WriteMessageToFile("Debug: GetCurrentMaxTime - MaxTime = NULL   da " + _server1 + "  a  " + _server2 + " Insert=" + " Insert= " + _isInsert.ToString() + " comando = " + _cmd.CommandText); 
                  
            }

            catch (Exception Ex)
            {
                MaxTime = _lastSyncTime;
                string action = string.Empty;
                if (_isInsert) action = "Operazione di inserimento";
                else action = "Operazione di Update";
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - Table.GetCurrentMaxTime - " +
                   Ex.Message + action + " Comando = " + _cmd.CommandText);
                
            }

            finally
            {
                if (_cmd.Connection.State == System.Data.ConnectionState.Open) _cmd.Connection.Close();
            }

            return MaxTime;

        }



        /// <summary>
        /// Elimina le righe definite come "cancellate"
        /// dalla coppia di tabelle presenti sui due server.
        /// </summary>
        /// <param name="_cmd">Comando sql</param>
        /// <param name="_serverName">Nome del server su cui eseguire il comando</param>        
        /// <param name="_lastSyncTime">Istante dell'ultima sincronizzazione</param>
        private void DelDeletedRows(SqlCommand _cmd, string _serverName,  System.Nullable<DateTime> _lastSyncTime)
        {
            int rowsAffected = 0;
            try
            {
                _cmd.CommandText = "DELETE [" + _serverName + "]." + this.completeName +
                                   " WHERE " + this.isDeletedColumnName + "=1";

                if (_lastSyncTime != null) _cmd.CommandText = _cmd.CommandText +
                                            " AND " + this.lastUpdateColumnName + " <= '" + ((DateTime)(_lastSyncTime)).ToString(CustomDateTime,
                                            System.Globalization.CultureInfo.InvariantCulture) + "'";


                _cmd.Connection.Open();
                rowsAffected = _cmd.ExecuteNonQuery();              
                                 
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - Table.DelDeletedRows - " + Ex.Message + " - Command: " + _cmd.CommandText);     
            }

            finally
            {
                if (_cmd.Connection.State == System.Data.ConnectionState.Open) _cmd.Connection.Close();
                
            }
        }
        #endregion PrivateMethod
        
        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (log != null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion

    }// END CLASS DEFINITION Table
}
