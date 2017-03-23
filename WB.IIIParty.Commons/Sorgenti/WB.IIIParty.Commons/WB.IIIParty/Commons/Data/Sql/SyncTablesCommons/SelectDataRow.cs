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
using WB.IIIParty.Commons.Data;
 
namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{
    /// <summary>
    /// Rappresenta le righe che devono essere estratte
    /// </summary>
    public class SelectDataRow : Row
    {

        #region PublicField

        /// <summary>
        /// Elenco delle colonne oggetto dell'istruzione select
        /// </summary>
        public List<string> columnsNameToSelect;

        /// <summary>
        /// Elenco delle colonne presenti nella condizione di "where"
        /// </summary>
        public List<string> columnsNameCondition;

        /// <summary>
        /// Elenco dei valori presenti nella condizione di "where"
        /// </summary>
        public List<object> valuesCondition;

        /// <summary>
        /// Elenco degli operatiri di confronto presenti nella condizione
        /// di "where"
        /// </summary>
        public List<ComparisonOperatorEnum> comparisonOperator;

        /// <summary>
        /// Elenco degli operatori logici presenti nella condizione
        /// di "where"
        /// </summary>
        public List<LogicalOperatorEnum> logicalOperator;

        /// <summary>
        /// Tipi di dato delle colonne
        /// </summary>
        public List<Type> columnsType;

        /// <summary>
        ///Numero di righe della clausola TOP 
        /// </summary>
        public int rowNumber;

        /// <summary>
        ///Clausola ORDER BY 
        /// </summary>
        public List<OrderClause> orderClause;

        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_dbName">Nome del database</param>
        /// <param name="_schema">Nome dello schema</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToSelect"> Elenco delle colonne oggetto dell'istruzione select</param>
        /// <param name="_columnsNameCondition">Elenco delle colonne presenti nella condizione di "where"</param>
        /// <param name="_valuesCondition">Elenco dei valori presenti nella condizione di "where"</param>
        /// <param name="_comparisonOperator">Elenco degli operatiri di confronto presenti nella condizione
        /// di "where"</param>
        /// <param name="_columnsType"></param>
        /// <param name="_logicalOperator">Elenco degli operatori logici presenti nella condizione
        /// di "where"</param>
        public SelectDataRow(string _dbName, string _schema, string _tableName,
                               List<string> _columnsNameToSelect,
                               List<string> _columnsNameCondition,
                               List<Type>   _columnsType,
                               List<object> _valuesCondition,
                               List<ComparisonOperatorEnum> _comparisonOperator,
                               List<LogicalOperatorEnum> _logicalOperator
                            )
            :base (_dbName, _schema, _tableName)
        {
            this.columnsNameToSelect = _columnsNameToSelect;
            this.columnsNameCondition =_columnsNameCondition;
            this.valuesCondition = _valuesCondition;
            this.comparisonOperator = _comparisonOperator;
            this.logicalOperator = _logicalOperator;
            this.columnsType = _columnsType;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_dbName">Nome del database</param>
        /// <param name="_schema">Nome dello schema</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToSelect"> Elenco delle colonne oggetto dell'istruzione select</param>
        /// <param name="_columnsNameCondition">Elenco delle colonne presenti nella condizione di "where"</param>
        /// <param name="_valuesCondition">Elenco dei valori presenti nella condizione di "where"</param>
        /// <param name="_comparisonOperator">Elenco degli operatiri di confronto presenti nella condizione di "where"</param>
        /// <param name="_columnsType">Elenco dei tipi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici presenti nella condizione di "where"</param>
        /// <param name="_n">Numero di righe per la clausola TOP</param>
        /// <param name="_clause">Clausola ORDER BY</param>
        public SelectDataRow(string _dbName, string _schema, string _tableName,
                               List<string> _columnsNameToSelect,
                               List<string> _columnsNameCondition,
                               List<Type> _columnsType,
                               List<object> _valuesCondition,
                               List<ComparisonOperatorEnum> _comparisonOperator,
                               List<LogicalOperatorEnum> _logicalOperator, 
                               int _n,
                               List<OrderClause> _clause
                            )
            : base(_dbName, _schema, _tableName)
        {
            this.columnsNameToSelect = _columnsNameToSelect;
            this.columnsNameCondition = _columnsNameCondition;
            this.valuesCondition = _valuesCondition;
            this.comparisonOperator = _comparisonOperator;
            this.logicalOperator = _logicalOperator;
            this.columnsType = _columnsType;
            this.rowNumber = _n;
            this.orderClause = _clause;
        }


        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_dbName">Nome del database</param>
        /// <param name="_schema">Nome dello schema</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToSelect"> Elenco delle colonne oggetto dell'istruzione select</param>
        /// <param name="_columnsNameCondition">Elenco delle colonne presenti nella condizione di "where"</param>
        /// <param name="_valuesCondition">Elenco dei valori presenti nella condizione di "where"</param>
        /// <param name="_comparisonOperator">Elenco degli operatiri di confronto presenti nella condizione
        /// di "where"</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici presenti nella condizione
        /// di "where"</param>
        public SelectDataRow(string _dbName, string _schema, string _tableName,
                               List<string> _columnsNameToSelect,
                               List<string> _columnsNameCondition,
                               List<object> _valuesCondition,
                               List<ComparisonOperatorEnum> _comparisonOperator,
                               List<LogicalOperatorEnum> _logicalOperator
                            )
            : base(_dbName, _schema, _tableName)
        {
            this.columnsNameToSelect = _columnsNameToSelect;
            this.columnsNameCondition = _columnsNameCondition;
            this.valuesCondition = _valuesCondition;
            this.comparisonOperator = _comparisonOperator;
            this.logicalOperator = _logicalOperator;
            this.columnsType = new List<Type>();
        }

     

        #endregion Constructor

    }// END CLASS DEFINITION SelectDataRow
}
