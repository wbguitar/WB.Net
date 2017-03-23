// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WB.IIIParty.Commons.Data
{
    /// <summary>
    /// 
    /// </summary>
    public struct OrderClause
    {
        /// <summary>
        /// Nome della colonna su cui fare l'ordinamento
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// Direzione dell'ordinamento
        /// </summary>
        public DirectionEnum Direction;
    }

    /// <summary>
    /// Enumera le direzioni possibili della clausola ORDER BY
    /// </summary>
    public enum DirectionEnum
    { 
        /// <summary>
        /// Discendente
        /// </summary>
        DESC,

        /// <summary>
        /// Ascendente
        /// </summary>
        ASC
    }

    /// <summary>
    /// Enumera un elenco di operatori di comparazione
    /// </summary>
    public enum ComparisonOperatorEnum
    {
        /// <summary>
        /// Uguale
        /// </summary>
        Equals,
        /// <summary>
        /// Maggiore
        /// </summary>
        Greater,
        /// <summary>
        /// Minore
        /// </summary>
        Less,
        /// <summary>
        /// Maggiore uguale
        /// </summary>
        GreaterEqual,
        /// <summary>
        /// Minore uguale
        /// </summary>
        LessEqual,
        /// <summary>
        /// Diverso
        /// </summary>
        NotEqual,
        /// <summary>
        /// Is [null]
        /// </summary>
        Is,
        /// <summary>
        /// Not is [null]
        /// </summary>
        NotIs
    }
    /// <summary>
    /// Enumera un elenco di operatori logici
    /// </summary>
    public enum LogicalOperatorEnum
    {
        /// <summary>
        /// And 
        /// </summary>
        AND,
        /// <summary>
        /// Or
        /// </summary>
        OR,
        /// <summary>
        /// Nessun Operatore
        /// </summary>
        NONE
    }

    /// <summary>
    /// Definisce l'interfaccia delle operazioni eseguibili su una tabella generica
    /// </summary>
    public interface ICommandToTable
    {
        /// <summary>
        /// Inserisce una riga su una taballa in un database
        /// </summary>
        /// <param name="_dbName">Nome del Database</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameList">Lista dei Nomi delle colonne</param>
        /// <param name="_values">Lista dei valori</param>
        /// <returns></returns>
        bool Insert(string _dbName, string _schema,string _tableName,
            List<string> _columnsNameList,List<object> _values);
       
        
         /// <summary>
        /// Inserisce una riga nella tabella in modo sincrono
        /// </summary>
        /// <param name="_dbName">Nome database.</param>
        /// <param name="_schema">Schema di apparteneza della tabella.</param>
        /// <param name="_tableName">Nome tabella.</param>
        /// <param name="_columnsNameList">Elenco delle colonne oggetto della insert.</param>
        /// <param name="_values">Valori da inserire.</param>
        /// <returns>True in caso di esito positivo, False altrimenti</returns>
        bool InsertSync(string _dbName, string _schema, string _tableName,
                           List<string> _columnsNameList, List<object> _values
                           );
        
        
        /// <summary>
        /// Consente la modifica di valori di una riga esistente in una tabella in un database
        /// </summary>
        /// <param name="_dbName">Nome del Database</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToUpdate">Elenco dei nomi delle colonne della tabella che devono essere aggiornate</param>
        /// <param name="_newvalues">Elenco nuovi valori</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione UPDATE</param>
        /// <param name="_valuesCondition">Elenco dei valori delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione UPDATE</param>
        /// <param name="_comparisonOperator">Elenco delle operazioni di comparazione tra la condizione ed il valore</param>
        /// <param name="_logicalOperator">Elenco delle operazioni logiche tra condizioni</param>
        bool Update(string _dbName, string _schema, string _tableName,
            List<string> _columnsNameToUpdate, List<object> _newvalues,
            List<string> _columnsNameCondition, List<object> _valuesCondition,
            List<ComparisonOperatorEnum> _comparisonOperator,
            List<LogicalOperatorEnum> _logicalOperator);
        
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
        bool UpdateSync(string _dbName, string _schema, string _tableName,
                List<string> _columnsNameToUpdate, List<object> _newvalues,
                List<string> _columnsNameCondition, List<object> _valuesCondition,
                List<ComparisonOperatorEnum> _comparisonOperator,
                List<LogicalOperatorEnum> _logicalOperator);
        
        
        /// <summary>
        /// Consente la cancellazione di righe da una tabella in un database
        /// </summary>
        /// <param name="_dbName">Nome del Database</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione DELETE</param>
        /// <param name="_valuesCondition">Elenco dei valori delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione DELETE</param>
        /// <param name="_comparisonOperator">Elenco delle operazioni di comparazione tra la condizione ed il valore</param>
        /// <param name="_logicalOperator">Elenco delle operazioni logiche tra condizioni</param>
        bool Delete(string _dbName, string _schema, string _tableName, 
            List<string> _columnsNameCondition, List<object> _valuesCondition,
            List<ComparisonOperatorEnum> _comparisonOperator,
            List<LogicalOperatorEnum> _logicalOperator);
            
                
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
        bool DeleteSync(string _dbName, string _schema, string _tableName,
                           List<string> _columnsNameCondition, List<object> _valuesCondition,
                           List<ComparisonOperatorEnum> _comparisonOperator,
                           List<LogicalOperatorEnum> _logicalOperator);

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
        DataTable Select(string _dbName, string _schema, string _tableName,
            List<string> _columnsNameToSelect, List<string> _columnsNameCondition,List<Type> _columnsType,
            List<object> _valuesCondition, List<ComparisonOperatorEnum> _comparisonOperator,
            List<LogicalOperatorEnum> _logicalOperator);

        /// <summary>
        /// Consente la lettura di una tabella
        /// </summary>
        /// <param name="_dbName">Nome del Database</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_tableName">Nome della tabella</param>
        /// <param name="_columnsNameToSelect">Elenco delle colonne da Selezionare</param>
        /// <param name="_columnsNameCondition">Elenco dei nomi delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_valuesCondition">Elenco dei valori delle colonne da utilizzare per costruire le condizioni di WHERE dell'istruzione SELECT</param>
        /// <param name="_comparisonOperator">Elenco delle operazioni di comparazione tra la condizione ed il valore</param>
        /// <param name="_logicalOperator">Elenco delle operazioni logiche tra condizioni</param>        
        /// <returns>Dati letti</returns>    
        DataTable Select(string _dbName, string _schema, string _tableName,
            List<string> _columnsNameToSelect, List<string> _columnsNameCondition,
            List<object> _valuesCondition, List<ComparisonOperatorEnum> _comparisonOperator,
            List<LogicalOperatorEnum> _logicalOperator);


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
        /// <param name="_n">Numero di righe per la clausola TOP (0 se non c'è la clausola TOP)</param>
        /// <param name="_clause">Elenco dei campi per la clausola ORDER BY</param>
        /// <returns>Dati letti</returns>
        DataTable Select(string _dbName, string _schema, string _tableName,
                                List<string> _columnsNameToSelect,
                                List<string> _columnsNameCondition, List<Type> _columnsType,
                                List<object> _valuesCondition,
                                List<ComparisonOperatorEnum> _comparisonOperator,
                                List<LogicalOperatorEnum> _logicalOperator,
                                int _n,
                                List<OrderClause> _clause);
    }
}
