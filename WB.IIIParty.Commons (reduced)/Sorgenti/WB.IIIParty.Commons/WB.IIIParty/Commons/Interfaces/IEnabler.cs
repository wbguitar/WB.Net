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

namespace WB.IIIParty.Commons.Interfaces
{
    /// <summary>
    /// Definisce l'interfaccia di Abilitazione e Disabilitazione delle funzionalità di una classe
    /// </summary>
    public interface IEnabler
    {
        /// <summary>
        /// Abilita le funzionalità dell'oggetto
        /// </summary>
        /// <returns></returns>
        void Enable();
        /// <summary>
        /// Disabilita le funzionalità dell'oggetto
        /// </summary>
        /// <returns></returns>
        void Disable();
        /// <summary>
        /// Ritorna l'abilitazione delle funzionalità dell'oggetto
        /// </summary>
        /// <returns></returns>
        bool IsEnable();
        /// <summary>
        /// Inizializza l'oggetto per rendere attivabili le funzionalità
        /// </summary>
        /// <returns></returns>
        void Initialize();
    }
}
