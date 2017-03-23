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

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{
    
    /// <summary>
    /// Eccezione di TimeOut per connessione al server
    /// </summary>
    public class TimeOutServerInfoException: Exception
    {
        /// <summary>
        /// Messaggio di errore
        /// </summary>
        public string errorMessage;
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_message"></param>
        public TimeOutServerInfoException(string _message)
        {
             this.errorMessage = _message;
            
        }
    }
}
