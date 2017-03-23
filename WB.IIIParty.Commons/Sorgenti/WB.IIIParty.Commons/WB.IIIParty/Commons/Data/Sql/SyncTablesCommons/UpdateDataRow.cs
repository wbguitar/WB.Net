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
    /// Rappresenta una riga di tabella che deve essere aggiornata.
    /// </summary>
    public class UpdateDataRow : Row
    {
        #region PublicField
        /// <summary>
        /// Elenco delle colonne oggetto dell'aggiornamento
        /// </summary>
        public List<string> columnsNameToUpdate;

        /// <summary>
        /// Elenco dei nuovi valori
        /// </summary>
        public List<object> newValues;

        /// <summary>
        /// Elenco delle colonne presenti nella condizione di "where" 
        /// </summary>
        public List<string> columnsNameCondition;

        /// <summary>
        /// Elenco dei valori presenti nella condizione di "where"
        /// </summary>
        public List<object> valuesCondition;

        /// <summary>
        /// Elenco degli operatori di confronto presenti nella condizione di "where"
        /// </summary>
        public List<ComparisonOperatorEnum> comparisonOperator;

        /// <summary>
        /// Elenco degli operatori logici presenti nella condizione di "where"
        /// </summary>
        public List<LogicalOperatorEnum> logicalOperator;


        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_dbName">Nome del database</param>
        /// <param name="_schema">Nome dello schema</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToUpdate">Elenco delle colonne oggetto dell'aggiornamento</param>
        /// <param name="_newValues">Elenco dei nuovi valori</param>
        /// <param name="_columnsNameCondition">Elenco delle colonne presenti nella condizione di "where" </param>
        /// <param name="_valuesCondition">Elenco dei nuovi valori</param>
        /// <param name="_comparisonOperator">Elenco degli operatori di confronto presenti nella condizione di "where"</param>
        /// <param name="_logicalOperator">Elenco degli operatori logici presenti nella condizione di "where"</param>
        public UpdateDataRow(string _dbName,string _schema, string _tableName,
                               List<string> _columnsNameToUpdate,
                               List<object> _newValues,
                               List<string> _columnsNameCondition,
                               List<object> _valuesCondition,
                               List<ComparisonOperatorEnum> _comparisonOperator,
                               List<LogicalOperatorEnum> _logicalOperator)
            :base (_dbName, _schema, _tableName)
        {
            this.columnsNameToUpdate = _columnsNameToUpdate;
            this.newValues = _newValues;
            this.columnsNameCondition = _columnsNameCondition;
            this.valuesCondition = _valuesCondition;
            this.comparisonOperator = _comparisonOperator;
            this.logicalOperator = _logicalOperator;
        }

        #endregion Constructor

    }// END CLASS DEFINITION UpdateDataRow
}
