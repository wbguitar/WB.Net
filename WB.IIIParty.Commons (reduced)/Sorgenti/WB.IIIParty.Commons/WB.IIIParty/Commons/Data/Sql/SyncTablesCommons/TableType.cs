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
    /// Tipi di tabella gestiti
    /// </summary>
    public enum TableType
    {
        /// <summary>
        /// OnEnable
        /// </summary>
        OnEnable = 0,
        /// <summary>
        /// OnStart
        /// </summary>
        OnStart = 1,
        /// <summary>
        /// History
        /// </summary>
        History = 2,
        /// <summary>
        /// NoRedundant
        /// </summary>
        NoRedundant = 3
    }
}
