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
    /// Delega la ricezione di un messaggio asincrono
    /// </summary>
    /// <param name="msg">Messaggio ricevuto</param>
    /// <param name="error">Eventuale eccezione di validazione generata</param>
    /// <param name="extra">Dati extra inclusi alla send async</param>
    public delegate void dAsyncCallback(IMessage msg, MessageValidationException error,object extra);
}
