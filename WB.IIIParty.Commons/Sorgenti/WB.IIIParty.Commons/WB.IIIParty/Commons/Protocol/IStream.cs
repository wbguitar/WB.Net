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
    /// Interfaccia di scambio di un flusso di bytes bidirezionale
    /// </summary>
    public interface IStream
    {
        /// <summary>
        /// Evento che notifica il trace dei dati inviati e ricevuti
        /// </summary>
        event dOnTrace OnTrace;
        /// <summary>
        /// Evento di ricezione dati
        /// </summary>
        event dDataReceived DataReceived;
        /// <summary>
        /// Invia un array di dati
        /// </summary>
        /// <param name="msg"></param>
        void Send(byte[] msg);
    }
}
