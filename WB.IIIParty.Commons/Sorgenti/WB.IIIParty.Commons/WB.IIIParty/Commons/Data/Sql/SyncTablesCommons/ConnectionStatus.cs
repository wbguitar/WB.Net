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


namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons  
{

    /// <summary>
    /// Indica i possibili stati di connessione al server sql
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// Non raggiungibile
        /// </summary>
        Bad = 0,
        /// <summary>
        /// Raggiungibile
        /// </summary>
        Good = 1,
        /// <summary>
        /// Non conosciuto
        /// </summary>
        Unkown = 2
    }
}
