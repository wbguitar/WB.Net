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

namespace WB.IIIParty.Commons.Logger
{    

    /// <summary>
    /// Interfaccia generica di Log di Debug
    /// </summary>
    public interface IMessageLog
    {
        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante la funzione di log</param>
        /// <param name="message">Messaggio di log</param>
        void Log(LogLevels level, object caller, string message);

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="message">Messaggio di log</param>
        void Log(LogLevels level, string message);

        /// <summary>
        /// Imposta o Ritorna il livello di Log corrente
        /// </summary>
        LogLevels LogLevel { get; set; }

        /// <summary>
        /// Ritorna se il livello di log specificato è abilitato
        /// </summary>
        /// <param name="level">Livello di Log</param>
        /// <returns></returns>
        bool CanLog(LogLevels level);
    }
}
