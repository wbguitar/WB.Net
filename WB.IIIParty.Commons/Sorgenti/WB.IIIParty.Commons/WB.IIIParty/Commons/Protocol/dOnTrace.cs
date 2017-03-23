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

namespace WB.IIIParty.Commons.Protocol
{
    /// <summary>
    /// Definisce la direzione del flusso di bytes
    /// </summary>
    public enum TraceDirections 
    { 
        /// <summary>
        /// Flusso di dati in Input
        /// </summary>
        Input,
        /// <summary>
        /// Flusso di dati in Output
        /// </summary>
        Output
    };

    /// <summary>
    /// Delega la ricezione di un evento di Trace
    /// </summary>
    /// <param name="sender">Gestore del flusso di dati</param>
    /// <param name="direction">Direzione del flusso di dati</param>
    /// <param name="data">Flusso di dati scambiati</param>
    /// <param name="description">Dettaglio dell'evento di Trace</param>
    public delegate void dOnTrace(IStream sender,TraceDirections direction, byte[] data,string description);
}
