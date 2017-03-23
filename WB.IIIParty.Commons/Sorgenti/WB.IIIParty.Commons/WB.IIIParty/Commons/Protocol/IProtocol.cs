// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-12-05 14:45:25 +0100 (lun, 05 dic 2011) $
//Versione: $Rev: 49 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Protocol
{
    /// <summary>
    /// Enumera i risultati di  invio sincrono
    /// </summary>
    public enum SyncResult
    {
        /// <summary>
        /// Corretto
        /// </summary>
        Ok,
        /// <summary>
        /// Timeout
        /// </summary>
        Timeout,
        /// <summary>
        /// Invio sincrono non disponibile
        /// </summary>
        NotAvaiable
    }

    /// <summary>
    /// Enumera i risultati di  invio asincrono
    /// </summary>
    public enum ASyncResult
    {
        /// <summary>
        /// Corretto
        /// </summary>
        Ok,
        /// <summary>
        /// Invio asincrono non disponibile
        /// </summary>
        NotAvaiable
    }

    /// <summary>
    /// Definisce l'interfaccia di ricezione e invio di messaggi di un protocollo generico
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Notifica la ricezione di un messaggio
        /// Notifica la generazione di un errore di validazione di un messaggio quando il parametro error è diverso da null
        /// </summary>
        event dMessageReceived MessageReceived;
        /// <summary>
        /// Notifica la generazione di un errore di parsing
        /// </summary>
        event dOnMessageParseError OnMessageParseError;
        /// <summary>
        /// Invia un messaggio
        /// </summary>
        /// <param name="msg"></param>
        void Send(IMessage msg);
        /// <summary>
        /// Invia un messaggio request-response in modalità sincrona
        /// </summary>
        /// <param name="msgIn">Messaggio di richiesta</param>
        /// <param name="msgOut">Messaggi di risposta</param>
        /// <param name="vEx">Eccezioni di validazione</param>
        SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx);
        /// <summary>
        /// Invia un messaggio request-response in modalità sincrona
        /// </summary>
        /// <param name="msgIn">Messaggio di richiesta</param>
        /// <param name="msgOut">Messaggi di risposta</param>
        /// <param name="vEx">Eccezioni di validazione</param>
        /// <param name="timeout">Timeout ricezione risposta</param>
        SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx,TimeSpan timeout);
        /// <summary>
        /// Invia un messaggio request-response in modalità asincrona
        /// </summary>
        /// <param name="msg">Messaggio di richiesta</param>
        /// <param name="callback">Callback di risposta</param>
        /// <param name="extra">Include dati extra che vengono notificati alla callback</param>
        ASyncResult SendAsync(IMessage msg, dAsyncCallback callback, object extra);
    }
}
