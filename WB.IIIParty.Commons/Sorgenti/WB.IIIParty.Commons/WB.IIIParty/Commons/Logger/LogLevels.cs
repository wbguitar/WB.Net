﻿// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-10-24 16:32:49 +0200 (gio, 24 ott 2013) $
//Versione: $Rev: 199 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Enumera i livelli di Log
    /// </summary>
    public enum LogLevels {
        /// <summary>
        /// Log disabilitati
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Log di Errore
        /// </summary>
        Error = 1,
        /// <summary>
        /// Log di Warning
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Log Informativi
        /// </summary>
        Info = 3,
        /// <summary>
        /// Log di Debug
        /// </summary>
        Debug = 4, 
        /// <summary>
        /// Log Verbosi
        /// </summary>
        Trace = 5        
    }

}
