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
    /// nterfaccia per implementare gli avvisi sonori, con beep macchina o wave.
    /// </summary>
    public interface ISoundBeepLoop
    {
        /// <summary>
        /// Play avviso sonoro
        /// </summary>
        void Play();
        /// <summary>
        /// Stop avviso sonoro
        /// </summary>
        void Stop();
        /// <summary>
        /// Abilita audio avviso sonoro
        /// </summary>
        void EnableAudio();
        /// <summary>
        /// Disabilita audio avviso sonoro
        /// </summary>
        void DisableAudio();
        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();
        /// <summary>
        /// Ritorna lo stato dell'audio attivo o disattivo (True/False)
        /// </summary>
        bool IsActive { get; }
    }
}
