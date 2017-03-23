// ------------------------------------------------------------------------
//Società:              WB IIIParty
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

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{

    /// <summary>
    /// Rappresenta la tabella che contiene le 
    /// informazioni relative ai processi di sincronizzazione
    /// </summary>
    public class SyncTableProcess
    {
        #region PublicField

        /// <summary>
        /// Nome della tabella
        /// </summary>
        public string name = string.Empty;
 
        /// <summary>
        /// Nome della colonna che contiene il nome della tabella oggetto del 
        /// processo di sincronizzazione
        /// </summary>
        public string colTable = string.Empty; 
        
        /// <summary>
        /// Nome della colonna che contiene la data-ora della sincronizzazione
        /// </summary>
        public string colDateTimeInsert = string.Empty;

        /// <summary>
        /// Nome della colonna che contiene l'esito del processo di sincronizzazione         
        /// </summary>
        public string colDateTimeUpdate = string.Empty; 

        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_name">Nome della tabella</param>
        /// <param name="_colTable">Nome della colonna che contiene il nome 
        /// della tabella oggetto del processo di sincronizzazione</param>
        /// <param name="_colDateTimeInsert">Nome della colonna che contine 
        /// la data-ora della sincronizzazione dovuta ad insert</param>
        /// <param name="_colDateTimeUpdate">Nome della colonna che contine 
        /// la data-ora della sincronizzazione dovuta ad update</param>
        public SyncTableProcess(string _name, string _colTable, 
                                string _colDateTimeInsert, string _colDateTimeUpdate)
        {
            this.name = _name;
            this.colTable = _colTable;
            this.colDateTimeInsert = _colDateTimeInsert;
            this.colDateTimeUpdate = _colDateTimeUpdate;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        public SyncTableProcess()
        {
            this.name = null;
            this.colTable = null;
            this.colDateTimeInsert = null;
            this.colDateTimeUpdate = null;
        }
        #endregion Constructor
    }
}
