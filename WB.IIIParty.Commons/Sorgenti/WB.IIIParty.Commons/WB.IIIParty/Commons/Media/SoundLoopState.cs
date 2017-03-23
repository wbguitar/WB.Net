// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Papi Rudy
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Media
{
    /// <summary>
    /// SoundLoopState memorizza lo stato da salvare per audio attivo o non attivo.
    /// </summary>
    public class SoundLoopState
    {
        /// <summary>
        /// Oggetto per la memorizzazione dello stato dell'audio, attivo o disattivo.
        /// </summary>
        public SoundLoopState()
        {
        }
        /// <summary>
        /// Stato audio attivo disattivo.
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }

    }
}
