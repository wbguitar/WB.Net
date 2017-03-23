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
    public interface ILoggerConfig
    {
        /// <summary>
        /// Nome del logger.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Implementa l'interfaccia per creare un instanza dell'oggetto logger.
        /// </summary>
        /// <returns></returns>
        object Create();

    }
}
