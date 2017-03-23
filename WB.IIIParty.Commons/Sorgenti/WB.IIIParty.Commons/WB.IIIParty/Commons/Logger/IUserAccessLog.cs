// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2012-10-09 15:28:09 +0200 (mar, 09 ott 2012) $
//Versione: $Rev: 114 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Enumera gli esiti di autenticazione
    /// </summary>
    public enum UserAccessResults
    {
        /// <summary>
        /// LogIn eseguito
        /// </summary>
        LogInOk = 0,
        /// <summary>
        /// Errore non specificato durante l'autenticazione
        /// </summary>
        LogInException = 1,
        /// <summary>
        /// Utente errato
        /// </summary>
        LogInUserError = 2,
        /// <summary>
        /// Password errata
        /// </summary>
        LogInPasswordError = 3,
        /// <summary>
        /// Utente già loggato al sistema
        /// </summary>
        LogInUserAlreadyLogger = 4,
        /// <summary>
        /// Un altro amministratore già loggato al sistema
        /// </summary>
        LogInAdministratorAlreadyLogged = 5,
        /// <summary>
        /// LogOut eseguito
        /// </summary>
        LogOutOk = 6,
        /// <summary>
        /// Errore non specificato durante l'autenticazione
        /// </summary>
        LogOutException = 7

    }

    /// <summary>
    /// Interfaccia generica di Log dell'esito di autenticazione utente
    /// </summary>
    public interface IUserAccessLog
    {

        /// <summary>
        /// Inserisce un log di esito di una autenticazione utente
        /// </summary>
        /// <param name="userName">Nome utente</param>
        /// <param name="group">Gruppo di appartenenza</param>
        /// <param name="workstationInfo">Postazione di accesso dell'operatore</param>
        /// <param name="applicationInfo"></param>
        /// <param name="result">Esito dell'operazione di autenticazione</param>
        void Log(string userName, int group,string workstationInfo,string applicationInfo, UserAccessResults result);

        /// <summary>
        /// Inserisce un log di esito di una autenticazione utente
        /// </summary>
        /// <param name="userName">Nome utente</param>
        /// <param name="group">Gruppo di appartenenza</param>
        /// <param name="workstationInfo">Postazione di accesso dell'operatore</param>
        /// <param name="applicationInfo"></param>
        /// <param name="result">Esito dell'operazione di autenticazione</param>
        void Log(string userName, string group, string workstationInfo, string applicationInfo, UserAccessResults result);
        
    }
}
