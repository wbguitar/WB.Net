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
    /// Enumera le tipologie di messaggio
    /// </summary>
    public enum MessageTypes
    {
        /// <summary>
        /// Sincronizzazione dei messaggi non supportata
        /// </summary>
        SyncNotSupported,
        /// <summary>
        /// Richiesta
        /// </summary>
        Request,
        /// <summary>
        /// Risposta
        /// </summary>
        Response,
        /// <summary>
        /// Notifica
        /// </summary>
        Notification
    }

    /// <summary>
    /// Definisce l'interfaccia di un messaggio
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Serializza il messaggio corrente in un array di bytes
        /// </summary>
        /// <returns></returns>
        byte[] GetByteArray();
        /// <summary>
        /// Effettua una validazione del contenuto del messaggio
        /// Errori di validazione devono essere generati ereditando da MessageValidationException
        /// </summary>
        void Validate();
        /// <summary>
        /// Ritorna una stringa contenente la stampa dei campi del messaggio
        /// </summary>
        string Display();
        /// <summary>
        /// Ritorna la tipologia di messaggio
        /// </summary>
        MessageTypes MessageType {get;}
        /// <summary>
        /// Ritorna il riferimento di sincronizzazione
        /// </summary>
        object SyncRef { get; }
        /// <summary>
        /// Ritorna il numero di messaggi di tipologia Response attesi dopo l'invio di un messaggio di tipo Request
        /// </summary>
        int ResponseCount { get; }
    }
}
