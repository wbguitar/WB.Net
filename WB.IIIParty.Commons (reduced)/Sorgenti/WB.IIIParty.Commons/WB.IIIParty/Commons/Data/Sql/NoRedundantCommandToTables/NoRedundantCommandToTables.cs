// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina
//Nome modulo software: CommandToTables.dll
//Data ultima modifica: $LastChangedDate: 2013-12-23 10:07:30 +0100 (lun, 23 dic 2013) $
//Versione:             $Rev: 210 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using WB.IIIParty.Commons.Data;
using WB.IIIParty.Commons.Data.Sql.SyncTablesCommons;
using WB.IIIParty.Commons.Data.Sql.NoRedundantTables;
using WB.IIIParty.Commons.Collections;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;

namespace WB.IIIParty.Commons.Data.Sql.NoRedundantCommandToTables
{
    
    /// <summary>
    /// Gestisce le operazioni di aggiornamento e select
    /// di dati presenti su tabelle di database ridondati o no.
    /// </summary>
    public class NoRedundantCommandToTables : IDisposable, ICommandToTable
    {
        #region Static
        private static string CustomDateTime = "yyyyMMdd HH:mm:ss.fff";
        private static OperatorConvert ConverterEnum = new OperatorConvert();
        #endregion Static        

        #region PrivateField  
        private ProdConsQueueEx msQueue;
        //private Dictionary<string, NoRedundantTable> tables;
        //private Dictionary<string, NoRedundantView> views;

        private SingleServerInfo server;
        private int capacityQueue;
        private IMessageLog log;
        private uint controllerConnectionTimer = 60;

        private object syncLock = new object();
        private bool resultUpdateDb;
    
        private Boolean pooling;

        #endregion 

        #region Constructor

        /// <summary>
        /// Permette di eseguire operazioni sul database.
        /// </summary>
        /// <param name="_server">Inforamzioni di configurazione sul server</param>
        /// <param name="_capacityQueue">Numero massimo di elementi gestiti dalla coda.</param>
        /// <param name="_pooling">Abilita o disabilita il pooling</param>
        public NoRedundantCommandToTables(SingleServerInfo _server,
                                 int _capacityQueue, Boolean _pooling)
        {
            try
            {
                log = LoggerToFile.LoggerToFile_singleton.GetLogger();
                this.capacityQueue = _capacityQueue;
                this.pooling = _pooling;
                this.msQueue = new ProdConsQueueEx(_capacityQueue, new ProdConsQueueEx.dObjectDequeue(WriteToDb));
                this.server = _server;
                //this.tables = new Dictionary<string, NoRedundantTable>();
                //this.views = new Dictionary<string, NoRedundantView>();



                if (this.server != null)
                {
                    this.server.EnableControllerConnection(controllerConnectionTimer);
                    this.server.WaitForReady(10000);
                }

                //this.GetAllTables();
                //this.GetAllViews();
            }
            catch (TimeOutServerInfoException Ex)
            {
                this.Log(LogLevels.Error, "NoRedundantCommandToTables Constructor - " + Ex.ToString());
                this.Dispose();
                throw new TimeOutServerInfoException(Ex.errorMessage);
            }
            catch (Exception Ex)
            {
                this.Log(LogLevels.Error, "NoRedundantCommandToTables Constructor - " + Ex.ToString());
                this.Dispose();
                throw new Exception("", Ex);
            }
        }

        #endregion Constructor

        #region PublicMethod

        /// <summary>
        /// Metodo di utilizzo di messageLog.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logMessage"></param>
        /// 
        private void Log(LogLevels logLevel, string logMessage)
        {
            if (this.log != null)
            {
                this.log.Log(logLevel, logMessage);
            }
        }   

        /// <summary>
        /// Inserisce la riga _row nella tabella.
        /// </summary>
        /// <param name="_row">Riga da inserire nella tabella.</param>
        private bool Insert(InsertDataRow _row)
        {
            return this.msQueue.Enqueue(_row);
        }

        /// <summary>
        /// Inserisce unariga nella tabella.
        /// </summary>
        /// <param name="_dbName">Nome database.</param>
        /// <param name="_schema">Schema di apparteneza della tabella.</param>
        /// <param name="_tableName">Nome tabella.</param>
        /// <param name="_columnsNameList">Elenco delle colonne oggetto della insert.</param>
        /// <param name="_values">Valori da inserire.</param>
        public bool Insert(string _dbName, string _schema, string _tableName,
                           List<string> _columnsNameList, List<object> _values
                          )
        {
            InsertDataRow row = new InsertDataRow(_dbName, _schema, _tableName,
                                                  _columnsNameList, _values);
            return this.msQueue.Enqueue(row);
        }

        /// <summary>
        /// Inserisce una riga nella tabella in modo sincrono
        /// </summary>
        /// <param name="_dbName">Nome database.</param>
        /// <param name="_schema">Schema di apparteneza della tabella.</param>
        /// <param name="_tableName">Nome tabella.</param>
        /// <param name="_columnsNameList">Elenco delle colonne oggetto della insert.</param>
        /// <param name="_values">Valori da inserire.</param>
        /// <returns>True in caso di esito positivo, False altrimenti</returns>
        public bool InsertSync(string _dbName, string _schema, string _tableName,
                           List<string> _columnsNameList, List<object> _values
                           )
        {
            InsertDataRow row = new InsertDataRow(_dbName, _schema, _tableName,
                                                  _columnsNameList, _values);

            lock (syncLock)
            {
                this.WriteToDb(row);
                return this.resultUpdateDb;
            }
        }


        /// <summary>
        /// Aggiorna i valori della riga _row.
        /// </summary>
        /// <param name="_row">Riga da aggiornare.</param>
        private bool Update(UpdateDataRow _row)
        {
            return this.msQueue.Enqueue(_row);
        }


        /// <summary>
        /// Esegue l'update di una riga.
        /// </summary>
        /// <param name="_dbName">Nome database</param>
        /// <param name="_schema">Nome schema</param>
        /// <param name="_tableName">Nome tabella</param>
        /// <param name="_columnsNameToUpdate">Elenco delle colonne da aggiornare</param>
        /// <param name="_newvalues">Elenco dei nuovi valori.</param>
        /// <param name="_columnsNameCondition">Elenco delle colonne da inserire
        /// nella Where</param>
        /// <param name="_valuesCondition">Elenco dei valori da inserire nella Where</param>
        /// <param name="_comparisonOperator">Elenco degli operatori di confronto 
        /// da utilizzare nella Where</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici da inserire nella Where</param>
        public bool Update(string _dbName, string _schema, string _tableName,
                List<string> _columnsNameToUpdate, List<object> _newvalues,
                List<string> _columnsNameCondition, List<object> _valuesCondition,
                List<ComparisonOperatorEnum> _comparisonOperator,
                List<LogicalOperatorEnum> _logicalOperator)
        {
            UpdateDataRow row = new UpdateDataRow(_dbName, _schema, _tableName,
                _columnsNameToUpdate, _newvalues, _columnsNameCondition,
                _valuesCondition, _comparisonOperator, _logicalOperator);
            return this.msQueue.Enqueue(row);
        }

        /// <summary>
        /// Esegue l'update di una riga in modo sincrono
        /// </summary>
        /// <param name="_dbName">Nome database</param>
        /// <param name="_schema">Nome schema</param>
        /// <param name="_tableName">Nome tabella</param>
        /// <param name="_columnsNameToUpdate">Elenco delle colonne da aggiornare</param>
        /// <param name="_newvalues">Elenco dei nuovi valori.</param>
        /// <param name="_columnsNameCondition">Elenco delle colonne da inserire
        /// nella Where</param>
        /// <param name="_valuesCondition">Elenco dei valori da inserire nella Where</param>
        /// <param name="_comparisonOperator">Elenco degli operatori di confronto 
        /// da utilizzare nella Where</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici da inserire nella Where</param>
        /// <returns> True in caso di esito positivo, False altrimenti</returns>
        public bool UpdateSync(string _dbName, string _schema, string _tableName,
                List<string> _columnsNameToUpdate, List<object> _newvalues,
                List<string> _columnsNameCondition, List<object> _valuesCondition,
                List<ComparisonOperatorEnum> _comparisonOperator,
                List<LogicalOperatorEnum> _logicalOperator)
        {
            UpdateDataRow row = new UpdateDataRow(_dbName, _schema, _tableName,
                _columnsNameToUpdate, _newvalues, _columnsNameCondition,
                _valuesCondition, _comparisonOperator, _logicalOperator);
            lock (syncLock)
            {
                this.WriteToDb(row);
                return this.resultUpdateDb;
            }
        }


        /// <summary>
        /// Cancella, logicamente, la riga _row dalla tabella.
        /// </summary>
        /// <param name="_row">Riga da cancellare.</param>
        private bool Delete(DeleteDataRow _row)
        {
            return this.msQueue.Enqueue(_row);
        }

        /// <summary>
        /// Elimina, logicamente, una riga dalla tabella.
        /// </summary>
        /// <param name="_dbName">Nome database.</param>
        /// <param name="_schema">Schema di appartenenza della tabella.</param>
        /// <param name="_tableName">Nome tabella.</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne 
        /// da utlizzare per la condizione di "where".</param>
        /// <param name="_valuesCondition">Elenco dei valori per creare la "where".</param>
        /// <param name="_comparisonOperator"> Elenco degli operatori di confronto per creare la "where"</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici per creare la condizione di "where"</param>
        public bool Delete(string _dbName, string _schema, string _tableName,
                           List<string> _columnsNameCondition, List<object> _valuesCondition,
                           List<ComparisonOperatorEnum> _comparisonOperator,
                           List<LogicalOperatorEnum> _logicalOperator)
        {
            DeleteDataRow row = new DeleteDataRow(_dbName, _schema, _tableName,
                _columnsNameCondition, _valuesCondition,
                _comparisonOperator, _logicalOperator);
            return this.msQueue.Enqueue(row);
        }

        /// <summary>
        /// Elimina, logicamente, una riga dalla tabella in modo sincrono
        /// </summary>
        /// <param name="_dbName">Nome database.</param>
        /// <param name="_schema">Schema di appartenenza della tabella.</param>
        /// <param name="_tableName">Nome tabella.</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne 
        /// da utlizzare per la condizione di "where".</param>
        /// <param name="_valuesCondition">Elenco dei valori per creare la "where".</param>
        /// <param name="_comparisonOperator"> Elenco degli operatori di confronto per creare la "where"</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici per creare la condizione di "where"</param>
        /// <returns>True in caso di esito positivo, False altrimenti</returns>
        public bool DeleteSync(string _dbName, string _schema, string _tableName,
                           List<string> _columnsNameCondition, List<object> _valuesCondition,
                           List<ComparisonOperatorEnum> _comparisonOperator,
                           List<LogicalOperatorEnum> _logicalOperator)
        {
            DeleteDataRow row = new DeleteDataRow(_dbName, _schema, _tableName,
                _columnsNameCondition, _valuesCondition,
                _comparisonOperator, _logicalOperator);

            lock (syncLock)
            {
                this.WriteToDb(row);
                return this.resultUpdateDb;
            }
        }

        /// <summary>
        /// Restituisce i valori della tabella _row.tableName.
        /// </summary>
        /// <param name="_row">Indica la tabella da dui estrarre i valori,
        /// le colonne e la condizione del filtro.</param>
        /// <returns>Valori della tabella.</returns>
        private DataTable Select(SelectDataRow _row)
        {
            return this.SelectFromDb(_row);
        }


        /// <summary>
        /// Estrae righe dalla tabella.
        /// </summary>
        /// <param name="_dbName">Nome del database.</param>
        /// <param name="_schema">Schema di appartenenza della tabella.</param>
        /// <param name="_tableName">Nome della tabella.</param>
        /// <param name="_columnsNameToSelect">Elenco delle colonne oggetto della "select".</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne 
        /// da utlizzare per la condizione di "where".</param>
        /// <param name="_valuesCondition">Elenco dei valori per creare la "where".</param>
        /// <param name="_comparisonOperator">Elenco degli operatori di confronto da inserire
        /// nella condizione di Where.</param>
        /// <param name="_logicalOperator">Elenco delgi operatori logici da inserire
        /// nella condizione di Where.</param>
        /// <returns></returns>
        public DataTable Select(string _dbName, string _schema, string _tableName,
                                List<string> _columnsNameToSelect,
                                List<string> _columnsNameCondition,
                                List<object> _valuesCondition,
                                List<ComparisonOperatorEnum> _comparisonOperator,
                                List<LogicalOperatorEnum> _logicalOperator
                                )
        {
            SelectDataRow row = new SelectDataRow(_dbName, _schema, _tableName,
                                    _columnsNameToSelect, _columnsNameCondition,
                                    _valuesCondition, _comparisonOperator, _logicalOperator);
            return this.SelectFromDb(row);
        }

        /// <summary>
        /// Consente la lettura di una tabella
        /// </summary>
        /// <param name="_dbName">Nome del Database</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToSelect">Elenco delle colonne da Selezionare</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_columnsType">Elenco dei tipi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_valuesCondition">Elenco dei valori delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_comparisonOperator">Elenco delle operazioni di comparazione tra la condizione ed il valore</param>
        /// <param name="_logicalOperator">Elenco delle operazioni logiche tra condizioni</param>        
        /// <returns>Dati letti</returns>    
        public DataTable Select(string _dbName, string _schema, string _tableName,
                                List<string> _columnsNameToSelect,
                                List<string> _columnsNameCondition, List<Type> _columnsType,
                                List<object> _valuesCondition,
                                List<ComparisonOperatorEnum> _comparisonOperator,
                                List<LogicalOperatorEnum> _logicalOperator
                                )
        {
            SelectDataRow row = new SelectDataRow(_dbName, _schema, _tableName,
                                    _columnsNameToSelect, _columnsNameCondition, _columnsType,
                                    _valuesCondition, _comparisonOperator, _logicalOperator);
            return this.SelectFromDb(row);
        }
        /// <summary>
        /// Consente la lettura di una tabella
        /// </summary>
        /// <param name="_dbName">Nome del Database</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToSelect">Elenco delle colonne da Selezionare</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_columnsType">Elenco dei tipi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_valuesCondition">Elenco dei valori delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_comparisonOperator">Elenco delle operazioni di comparazione tra la condizione ed il valore</param>
        /// <param name="_logicalOperator">Elenco delle operazioni logiche tra condizioni</param>  
        /// <param name="_n">Numero di righe per la clausola TOP</param>
        /// <param name="_clause">Clausola ORDER BY</param>
        /// <returns>Dati letti</returns>    
        public DataTable Select(string _dbName, string _schema, string _tableName,
                                List<string> _columnsNameToSelect,
                                List<string> _columnsNameCondition, List<Type> _columnsType,
                                List<object> _valuesCondition,
                                List<ComparisonOperatorEnum> _comparisonOperator,
                                List<LogicalOperatorEnum> _logicalOperator,
                                int _n,
                                List<OrderClause> _clause)
        {
            SelectDataRow row = new SelectDataRow(_dbName, _schema, _tableName,
                                    _columnsNameToSelect, _columnsNameCondition, _columnsType,
                                    _valuesCondition, _comparisonOperator, _logicalOperator, _n, _clause);
            return this.SelectFromDb(row);
        }
        #endregion PublicMethod

        #region PrivateMethod
        //private void GetAllViews()
        //{
        //    List<NoRedundantView> views = new List<NoRedundantView>();
        //    try
        //    {
        //        List<NoRedundantDatabase> dbs = this.server.GetLocalDbList(string.Empty, 0);

        //        foreach (NoRedundantDatabase currentDb in dbs)
        //        {
        //            views = currentDb.GetLocalViewList();

        //            foreach (NoRedundantView currentView in views)
        //            {
        //                this.views.Add(currentView.CompleteName, currentView);
        //            }
        //            currentDb.Dispose();
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
        //               " - CommandToTables - GetAllViews - " + Ex.Message);
        //    }
        //}

        //private void GetAllTablesFromDb(NoRedundantDatabase _currentDb)
        //{
        //    List<NoRedundantTable> tables = new List<NoRedundantTable>();
        //    try
        //    {
               
        //            tables = _currentDb.GetLocalTableList();

        //            foreach (NoRedundantTable currentTable in tables)
        //        {
        //            this.tables.Add(currentTable.CompleteName, currentTable);                  
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
        //               " - CommandToTables.GetAllTablesFromDb - " +
        //               Ex.Message);
        //    }
        //}

        //private void GetAllTables()
        //{
        //    try
        //    {
        //        //estrazione elenco tabelle locali
        //        List<NoRedundantDatabase> dbs = this.server.GetLocalDbList(string.Empty, 0);


        //        foreach (NoRedundantDatabase currentDb in dbs)
        //        {
        //            this.GetAllTablesFromDb(currentDb);                   
        //            currentDb.Dispose();                   
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
        //           " - CommandToTables - GetAllTables - " +
        //           Ex.Message);
        //    }
        //}

        private string GetExistsCondition(Row _currentRow, NoRedundantTable _tab)
        {
            string result = string.Empty;

            switch (_currentRow.GetType().Name)
            {
                case "InsertDataRow":
                    {
                        InsertDataRow row = new InsertDataRow(_currentRow.dbName,
                            _currentRow.schema, _currentRow.tableName,
                            ((InsertDataRow)_currentRow).columnsName,
                            ((InsertDataRow)_currentRow).values);

                        for (int i = 0; i < row.columnsName.Count; i++)
                            //if (_tab.primaryKeys.Contains(row.columnsName[i].ToUpper()))
                                if (result == string.Empty) result = row.columnsName[i].ToUpper() + "=" + this.GetValue(row.values[i]) + "";
                                else result = result + " AND " + row.columnsName[i].ToUpper() + "=" + this.GetValue(row.values[i]) + "";


                        break;
                    }

                case "UpdateDataRow":
                    {
                        UpdateDataRow row = new UpdateDataRow(_currentRow.dbName,
                            _currentRow.schema, _currentRow.tableName,
                            ((UpdateDataRow)_currentRow).columnsNameToUpdate,
                            ((UpdateDataRow)_currentRow).newValues,
                            ((UpdateDataRow)_currentRow).columnsNameCondition,
                            ((UpdateDataRow)_currentRow).valuesCondition,
                            ((UpdateDataRow)_currentRow).comparisonOperator,
                            ((UpdateDataRow)_currentRow).logicalOperator);

                        for (int i = 0; i < row.columnsNameCondition.Count; i++)
                            if (result == string.Empty) result = row.columnsNameCondition[i].ToUpper() +
                                                                 ConverterEnum.GetStringFromEnum(row.comparisonOperator[i]) + this.GetValue(row.valuesCondition[i]) +
                                                                  " " + ConverterEnum.GetStringFromEnum(row.logicalOperator[i]) + " ";
                            else result = result + row.columnsNameCondition[i].ToUpper() +
                                          ConverterEnum.GetStringFromEnum(row.comparisonOperator[i]) + this.GetValue(row.valuesCondition[i]) +
                                          " " + ConverterEnum.GetStringFromEnum(row.logicalOperator[i]) + " ";                        
                        break;
                    }
                case "DeleteDataRow":
                    {
                        if (
                            (((DeleteDataRow)_currentRow).columnsNameCondition != null) &&
                            (((DeleteDataRow)_currentRow).valuesCondition != null) &&
                            (((DeleteDataRow)_currentRow).comparisonOperator != null) &&
                            (((DeleteDataRow)_currentRow).logicalOperator != null)
                            )
                        {
                            DeleteDataRow row = new DeleteDataRow(_currentRow.dbName,
                                _currentRow.schema, _currentRow.tableName,
                                ((DeleteDataRow)_currentRow).columnsNameCondition,
                                ((DeleteDataRow)_currentRow).valuesCondition,
                                ((DeleteDataRow)_currentRow).comparisonOperator,
                                ((DeleteDataRow)_currentRow).logicalOperator);

                            for (int i = 0; i < row.columnsNameCondition.Count; i++)
                                if (result == string.Empty) result = row.columnsNameCondition[i].ToUpper() +
                                                            ConverterEnum.GetStringFromEnum(row.comparisonOperator[i]) + this.GetValue(row.valuesCondition[i])
                                                            + " " + ConverterEnum.GetStringFromEnum(row.logicalOperator[i]) + " ";
                                else result = result + row.columnsNameCondition[i].ToUpper() +
                                              ConverterEnum.GetStringFromEnum(row.comparisonOperator[i]) + this.GetValue(row.valuesCondition[i]) +
                                              " " + ConverterEnum.GetStringFromEnum(row.logicalOperator[i]) + " ";
                        }
                        else result = string.Empty;

                        break;
                    }

                case "SelectDataRow":
                    {
                        SelectDataRow row = new SelectDataRow(_currentRow.dbName,
                                _currentRow.schema, _currentRow.tableName,
                                ((SelectDataRow)_currentRow).columnsNameToSelect,
                                ((SelectDataRow)_currentRow).columnsNameCondition,
                                ((SelectDataRow)_currentRow).valuesCondition,
                                ((SelectDataRow)_currentRow).comparisonOperator,
                                ((SelectDataRow)_currentRow).logicalOperator
                                );

                        for (int i = 0; i < row.columnsNameCondition.Count; i++)
                            if (result == string.Empty) result = row.columnsNameCondition[i].ToUpper() +
                                                                 ConverterEnum.GetStringFromEnum(row.comparisonOperator[i]) + this.GetValue(row.valuesCondition[i]) +
                                                                 " " + ConverterEnum.GetStringFromEnum(row.logicalOperator[i]) + " ";
                            else result = result + row.columnsNameCondition[i].ToUpper() +
                                     ConverterEnum.GetStringFromEnum(row.comparisonOperator[i]) + this.GetValue(row.valuesCondition[i]) +
                                       " " + ConverterEnum.GetStringFromEnum(row.logicalOperator[i]) + " ";

                        break;
                    }

                default:
                    break;
            }


            return result;

        }

        private string GetUpdateCondition(Row _currentRow, NoRedundantTable _tab, string _existsCondition, DateTime _time)
        {
            string result = string.Empty;
            switch (_currentRow.GetType().Name)
            {
                case "InsertDataRow":
                    {
                        InsertDataRow row = new InsertDataRow(_currentRow.dbName,
                            _currentRow.schema, _currentRow.tableName,
                            ((InsertDataRow)_currentRow).columnsName,
                            ((InsertDataRow)_currentRow).values);


                        for (int i = 0; i < row.columnsName.Count; i++)
                            //if (!(_tab.primaryKeys.Contains(row.columnsName[i].ToUpper())))
                                if (result == string.Empty) result = row.columnsName[i].ToUpper() +
                                                            ConverterEnum.GetStringFromEnum(ComparisonOperatorEnum.Equals) + this.GetValue(row.values[i]);
                                else result = result + " , " + row.columnsName[i].ToUpper() +
                                     ConverterEnum.GetStringFromEnum(ComparisonOperatorEnum.Equals) + this.GetValue(row.values[i]);

                        result = result + " WHERE " + _existsCondition;

                        break;
                    }
                case "UpdateDataRow":
                    {
                        UpdateDataRow row = new UpdateDataRow(_currentRow.dbName,
                            _currentRow.schema, _currentRow.tableName,
                            ((UpdateDataRow)_currentRow).columnsNameToUpdate,
                            ((UpdateDataRow)_currentRow).newValues,
                            ((UpdateDataRow)_currentRow).columnsNameCondition,
                            ((UpdateDataRow)_currentRow).valuesCondition,
                            ((UpdateDataRow)_currentRow).comparisonOperator,
                            ((UpdateDataRow)_currentRow).logicalOperator);

                        for (int i = 0; i < row.columnsNameToUpdate.Count; i++)
                            //if (!(_tab.primaryKeys.Contains(row.columnsNameToUpdate[i].ToUpper())))
                                if (result == string.Empty) result = row.columnsNameToUpdate[i].ToUpper() +
                                                            ConverterEnum.GetStringFromEnum(ComparisonOperatorEnum.Equals) + this.GetValue(row.newValues[i]);
                                else result = result + " , " + row.columnsNameToUpdate[i].ToUpper() +
                                              ConverterEnum.GetStringFromEnum(ComparisonOperatorEnum.Equals) + this.GetValue(row.newValues[i]);

                        result = result + " WHERE " + _existsCondition;
                        break;
                    }

                case "DeleteDataRow":
                    {
                        if (
                            (((DeleteDataRow)_currentRow).columnsNameCondition != null) &&
                            (((DeleteDataRow)_currentRow).valuesCondition != null) &&
                            (((DeleteDataRow)_currentRow).comparisonOperator != null) &&
                            (((DeleteDataRow)_currentRow).logicalOperator != null)
                           )
                        {

                            DeleteDataRow row = new DeleteDataRow(_currentRow.dbName,
                                _currentRow.schema, _currentRow.tableName,
                                ((DeleteDataRow)_currentRow).columnsNameCondition,
                                ((DeleteDataRow)_currentRow).valuesCondition,
                                ((DeleteDataRow)_currentRow).comparisonOperator,
                                ((DeleteDataRow)_currentRow).logicalOperator);

                            for (int i = 0; i < row.columnsNameCondition.Count; i++)
                                //if (!(_tab.primaryKeys.Contains(row.columnsNameCondition[i].ToUpper())))
                                    if (result == string.Empty) result = row.columnsNameCondition[i].ToUpper() +
                                                                ConverterEnum.GetStringFromEnum(ComparisonOperatorEnum.Equals) + this.GetValue(row.valuesCondition[i]);
                                    else result = result + " , " + row.columnsNameCondition[i].ToUpper() +
                                                  ConverterEnum.GetStringFromEnum(ComparisonOperatorEnum.Equals) + this.GetValue(row.valuesCondition[i]);
                        }
                      
                        if (_existsCondition != string.Empty) result = result + " WHERE " + _existsCondition;

                        break;
                    }

                default:
                    break;
            }
            return result;
        }


        private string GetValue(object obj)
        {
            string result = string.Empty;
            if (obj == null) result = "null";
            else
                switch (obj.GetType().Name)
                {
                    case "DateTime":
                        {
                            result = "'" + (string)((DateTime)obj).ToString(CustomDateTime, System.Globalization.CultureInfo.InvariantCulture) + "'";
                            break;
                        }
                    case "String":
                        {
                            System.Text.StringBuilder b = new StringBuilder((string)obj);
                            b.Replace("'", "''");
                            result = "N'" + b.ToString() + "'";

                            break;
                        }
                    case "DBNull":
                        {
                            result = "null";
                            break;
                        }
                    case "Single":
                        {
                            return Convert.ToSingle(obj).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));
                        }
                    case "Double":
                        {
                            return Convert.ToDouble(obj).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));
                        }
                    case "Boolean":
                        {
                            if ((Boolean)obj) result = "'" + "true" + "'";
                            else result = "'" + "false" + "'";
                            break;
                        }
                    default:
                        {
                            result = obj.ToString();
                            break;
                        }
                }
            return result;
        }

        // Scrive sul database le informazioni contenute nel messaggio estratto dalla coda 
        private void WriteToDb(object _obj)
        {
            int result = 0;
            DateTime dateTimeUpdate = DateTime.Now.ToUniversalTime();

            lock (syncLock)
            {

                string completeTableName = ((Row)_obj).dbName + "." +
                                            ((Row)_obj).schema + "." +
                                            ((Row)_obj).tableName;
                
                //if ((this.tables!=null)&&(this.tables.ContainsKey(completeTableName.ToUpper())))
                //{
                    //NoRedundantTable tab = new NoRedundantTable(this.tables[completeTableName.ToUpper()].Db, this.tables[completeTableName.ToUpper()].Schema,
                    //                      this.tables[completeTableName.ToUpper()].Name, 
                    //                      this.tables[completeTableName.ToUpper()].primaryKeys,
                    //                      this.tables[completeTableName.ToUpper()].allColumns);
                NoRedundantDatabase  dataBase = new NoRedundantDatabase(this.server,((Row)_obj).dbName);
                NoRedundantTable tab = new NoRedundantTable(
                                                    dataBase, 
                                                    ((Row)_obj).schema,
                                                    ((Row)_obj).tableName);
                                                                      

                    string primaryCondition = string.Empty;
                    switch (_obj.GetType().Name)
                    {
                        case "InsertDataRow":
                            {
                                try
                                {
                                    string columns = string.Empty;
                                    string values = string.Empty;

                                    columns = this.ConcatenateString(((InsertDataRow)_obj).columnsName); 

                                    values = this.ConcatenateObject(((InsertDataRow)_obj).values);
                                    
                                    string cmdText = "INSERT INTO " + completeTableName + "(" + columns +
                                                        " ) VALUES (" + values + ")";

                                    if ((this.server != null) && (this.server.status == ConnectionStatus.Good))
                                        result = this.ExecuteCommand(this.server,
                                                      ((InsertDataRow)_obj).dbName, cmdText);                                    
                                    if (result > 0) this.resultUpdateDb = true;
                                    else this.resultUpdateDb = false;

                                }

                                catch (Exception Ex)
                                {
                                    this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                                     " - CommandToTables.WriteToDb - " +
                                     Ex.Message);
                                    this.resultUpdateDb = false;
                                }
                                finally
                                {

                                }
                                break;
                            }
                        case "UpdateDataRow":
                            {
                                try
                                {
                                    string existsCondition = this.GetExistsCondition(((UpdateDataRow)_obj), tab);
                                    string cmdText = "UPDATE " + completeTableName +
                                                     " SET " + this.GetUpdateCondition(((UpdateDataRow)_obj), tab, existsCondition, dateTimeUpdate);

                                    if ((this.server!= null) && (this.server.status == ConnectionStatus.Good))
                                        result = this.ExecuteCommand(this.server,
                                                      ((UpdateDataRow)_obj).dbName, cmdText);                                    
                                    if (result > 0) this.resultUpdateDb = true;
                                    else this.resultUpdateDb = false;
                                }
                                catch (Exception Ex)
                                {
                                    this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                                         " - CommandToTables.WriteToDb - " +
                                         Ex.Message);
                                    this.resultUpdateDb = false;
                                }
                                break;
                            }

                        case "DeleteDataRow":
                            {
                                try
                                {
                                    string existsCondition = this.GetExistsCondition(((DeleteDataRow)_obj), tab);
                                    string whereCondition = string.Empty;
                                    if (existsCondition != string.Empty) whereCondition = " WHERE " + existsCondition ;
                                    else whereCondition = ")";

                                    string cmdText = "DELETE FROM " +
                                                     completeTableName +                                        
                                                     whereCondition;
                                    
                                    if ((this.server != null) && (this.server.status == ConnectionStatus.Good))
                                        result = this.ExecuteCommand(this.server,
                                                      ((DeleteDataRow)_obj).dbName, cmdText);                                   
                                    if (result > 0) this.resultUpdateDb = true;
                                    else this.resultUpdateDb = false;
                                }
                                catch (Exception Ex)
                                {
                                    this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                                         " - CommandToTables.WriteToDb - " +
                                         Ex.Message);
                                    this.resultUpdateDb = false;
                                }

                                break;
                            }
                        default:

                            break;
                    }
                    dataBase.Dispose();
                    tab.Dispose();
                //}
                //else
                //{
                //    log.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                //                     " - CommandToTables.WriteToDb - Impossibile eseguire l'operazione nella tabella " +
                //                     completeTableName + ". La tabella non esiste nell'elenco estratto.");
                //    this.resultUpdateDb = false;
                //}
            }

        }

        private string ConcatenateString(List<string> _list)
        {
            string result = string.Empty;
            foreach (string s in _list)
                if (result == string.Empty) result = s.ToUpper();
                else result = result + "," + s.ToUpper();


            return result;
        }

        private string ConcatenateObject(List<object> _list)
        {
            string result = string.Empty;
            foreach (object s in _list)
                if (result == string.Empty) result = this.GetValue(s);
                else result = result + "," + this.GetValue(s);

            return result;
        }

        private int ExecuteCommand(SingleServerInfo _currentServer, string _dbName, string _command)
        {
            int result = 0;
            SqlCommand cmd = new SqlCommand();
            try
            {

                SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                conn.InitialCatalog = _dbName;
                conn.Password = _currentServer.password;
                conn.UserID = _currentServer.userName;
                conn.DataSource = _currentServer.localName;
                conn.Pooling = this.pooling;
                if(_currentServer.failoverPartner!=string.Empty) conn.FailoverPartner = _currentServer.failoverPartner;
                cmd.Connection = new SqlConnection(conn.ConnectionString);
                cmd.CommandText = _command;
                //Insert Timeout
                cmd.CommandTimeout = _currentServer.commandTimeOut;
                cmd.Connection.Open();
                result = cmd.ExecuteNonQuery();

            }

            catch (Exception Ex)
            {
                this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                         " - CommandToTables.ExecuteCommand - " +
                         Ex.Message + "  Comando: " + _command);
            }

            finally
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Open) cmd.Connection.Close();
                cmd.Dispose();
            }

            return result;
        }

        private DataTable SelectFromDb(SelectDataRow _row)
        {
            DataTable result = new DataTable();
            string completeTableName = _row.dbName + "." +
                                       _row.schema + "." +
                                       _row.tableName;
            //string isDeletedColumnName = string.Empty;
            NoRedundantDatabase dataBase = new NoRedundantDatabase(this.server, ((Row)_row).dbName);
            NoRedundantTable tab = new NoRedundantTable(
                                                dataBase,
                                                ((Row)_row).schema,
                                                ((Row)_row).tableName);

            string cmdText = string.Empty;       
            


                if (_row.rowNumber != 0)// Clausola TOP
                    cmdText = " SELECT TOP " + _row.rowNumber + " " + this.ConcatenateString(_row.columnsNameToSelect) + " FROM " + completeTableName;
                else
                    cmdText = " SELECT " + this.ConcatenateString(_row.columnsNameToSelect) +" FROM " + completeTableName;

                if ((_row.columnsNameCondition != null) && (_row.columnsNameCondition.Count > 0)) // Clausole di controllo dei vari campi
                    cmdText = cmdText + " WHERE " + " " + this.GetExistsCondition(_row, null);


                if ((_row.orderClause != null) && (_row.orderClause.Count > 0)) // Clausula ORDER BY
                {
                    cmdText = cmdText + " ORDER BY ";

                    for (int i = 0; i < _row.orderClause.Count - 1; i++)
                    {
                        cmdText = cmdText + _row.orderClause[i].ColumnName + " " + _row.orderClause[i].Direction.ToString() + ",";
                    }
                    cmdText = cmdText + _row.orderClause[_row.orderClause.Count - 1].ColumnName + " " + _row.orderClause[_row.orderClause.Count - 1].Direction.ToString();
                }


                SqlCommand cmd = new SqlCommand();
                try
                {
                    if (this.server.status == ConnectionStatus.Good)
                    {
                        SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                        conn.InitialCatalog = _row.dbName;
                        conn.Password = this.server.password;
                        conn.UserID = this.server.userName;
                        conn.DataSource = this.server.localName;
                        conn.Pooling = this.pooling;
                        if (server.failoverPartner != string.Empty) conn.FailoverPartner = server.failoverPartner;
                        cmd.Connection = new SqlConnection(conn.ConnectionString);
                        cmd.CommandText = cmdText;

                        for (int i = 0; i < _row.columnsNameToSelect.Count; i++)
                        {
                            string name = _row.columnsNameToSelect[i];

                            if (_row.columnsType.Count > i)
                            {
                                Type t = _row.columnsType[i];
                                result.Columns.Add(name, t);
                            }
                            else
                            {
                                result.Columns.Add(name);
                            }

                        }

                        object[] valuesSelected = new object[_row.columnsNameToSelect.Count];
                        cmd.Connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            reader.GetValues(valuesSelected);
                            result.Rows.Add(valuesSelected);
                        }
                    }
                }

                catch (Exception Ex)
                {
                    this.Log(LogLevels.Error, CustomTimeStamp.GetTimeStamp() +
                             " - CommandToTables.SelectFromDb - " +
                             Ex.Message);
                    
                    throw Ex;
                }
                finally
                {
                    if (cmd.Connection.State == System.Data.ConnectionState.Open) cmd.Connection.Close();
                    cmd.Dispose();                    
                }
                dataBase.Dispose();
                tab.Dispose();
            //}

            return result;
        }
        #endregion PrivateMethod

        #region IDisposable Members
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //if (this.tables != null)
            //{
            //    foreach (NoRedundantTable t in this.tables.Values)
            //    {
            //        if (t != null)
            //        {
            //            t.Dispose();
            //        }
            //    }
            //    this.tables = null;
            //}

            if (this.server != null)
            {
                this.server.Dispose();
                this.server = null;
            }

            if (this.msQueue != null)
            {
                this.msQueue.Dispose();
                this.msQueue = null;
            }

            if (log != null)
                LoggerToFile.LoggerToFile_singleton.Dispose();
        }
        #endregion

    }
}
