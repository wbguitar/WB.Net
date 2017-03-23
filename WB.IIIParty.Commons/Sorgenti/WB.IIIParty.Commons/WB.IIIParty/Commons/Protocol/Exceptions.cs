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
    /// Delega la ricezione di un evento OnMessageParseError
    /// </summary>
    /// <param name="error">Errore di Parsing generato</param>
    public delegate void dOnMessageParseError(MessageParseException error);
    /// <summary>
    /// Eccezione di Parsing di un messaggio
    /// </summary>
    public class MessageParseException : Exception 
    { 
        /// <summary>
        /// Ctr1
        /// </summary>
        public MessageParseException() : base() {}
        /// <summary>
        /// Ctr1
        /// </summary>
        public MessageParseException(string message) : base(message) { }
    }
    /// <summary>
    /// Eccezione di Validazione di un messaggio
    /// </summary>
    public class MessageValidationException : Exception
    {
        /// <summary>
        /// Ctr1
        /// </summary>
        public MessageValidationException() : base() {}
        /// <summary>
        /// Ctr2
        /// </summary>
        public MessageValidationException(string message) : base(message) { }
    }
}
