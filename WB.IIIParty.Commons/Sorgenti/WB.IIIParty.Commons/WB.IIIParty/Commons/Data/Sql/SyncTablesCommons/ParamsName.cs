// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5
//Autore:               Marziali Valentina
//Nome modulo software: SyncTablesCommons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $ 
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{ 

    /// <summary>
    /// Elenco dei parametri da leggere dalla tabella dei Parametri
    /// </summary>
    public class ParamsName
    {
        /// <summary>
        /// Periodi di sincronizzazione espresso in secondi
        /// </summary>
        public const string Interval = "Interval";
        /// <summary>
        /// Nome della proprietà estesa che indica se il database/tabella 
        /// è soggetta al processo di sincronizzazione
        /// </summary>
        public const string Synchronize = "Synchronize";
        /// <summary>
        /// Nome della proprietà estesa che indica il tipo della tabella
        /// </summary>
        public const string Type = "Type";
        /// <summary>
        /// Nome della tabella che registra gli eventi di sincronizzazione
        /// </summary>
        public const string TableSync = "TableSync";
        /// <summary>
        /// Nome della colonna, della tabella TableSync, che contiene
        /// il nome delle tabelle da sincronizzare
        /// </summary>
        public const string TableName ="TableName";
        /// <summary>
        /// Nome della colonna, della tabella TableSync, che contiene la data-ora
        /// della sincronizzazione con esito positivo dovuta ad insert
        /// </summary>
        public const string DateTimeNameInsert = "DateTimeNameInsert";
        /// <summary>
        /// Nome della colonna, della tabella TableSync, che contiene la data-ora
        /// della sincronizzazione con esito positivo dovuta ad update
        /// </summary>
        public const string DateTimeNameUpdate = "DateTimeNameUpdate";
        /// <summary>
        /// Numero massimo di linee da prendere in considerazione durante il
        /// processo di sincronizzazione periodico
        /// </summary>
        public const string MaxRows = "MaxRows";
        /// <summary>
        /// Nome della proprietà estesa che indica il della colonna
        /// Cancellata
        /// </summary>
        public const string IsDeleted = "IsDeleted";
        /// <summary>
        /// Nome della proprietà estesa che indica il della colonna
        /// dell'istante dell'ultimo aggiornamento della riga
        /// </summary>
        public const string LastUpdate = "LastUpdate";
        /// <summary>
        /// Intervallo di tempo, espresso in secondi, che intercorre tra due
        /// verifiche consecutive di raggiungibilità del server
        /// </summary>
        public const string IntervalConnectionController = "IntervalConnectionController";
    }

}
