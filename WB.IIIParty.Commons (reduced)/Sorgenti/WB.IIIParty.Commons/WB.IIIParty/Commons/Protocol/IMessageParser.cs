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
    /// Definisce l'interfaccia di Parsing di un messaggio necessaria a StreamParser per la decodifica del flusso di dati
    /// </summary>
    public interface IMessageParser
    {
        /// <summary>
        /// Ritorna se dai dati a disposizione è possibile calcolare la lunghezza COMPLESSIVA del messaggio
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool CanReadLength(byte[] data);
        /// <summary>
        /// Ritorna la lunghezza COMPLESSIVA del messaggio
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int GetLength(byte[] data);
        /// <summary>
        /// Effettua la deserializzazione di un messaggio da un array di byte
        /// Errori di parsing devono essere generati ereditando da MessageParseException
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IMessage ParseMessage(byte[] data);
        /// <summary>
        /// Effettua la serializzazione di un messaggio in un array di byte
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] SerializeMessage(IMessage data);
        /// <summary>
        /// Ritorna se la classe può serializzare un oggetto
        /// </summary>
        /// <returns></returns>
        bool SerializeIsSupported();
    }
}
