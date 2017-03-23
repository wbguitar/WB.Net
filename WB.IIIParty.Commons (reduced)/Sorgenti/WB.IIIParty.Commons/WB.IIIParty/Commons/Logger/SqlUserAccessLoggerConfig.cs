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
using WB.IIIParty.Commons.Data;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa un LoggerConfig per il logger SqlLogger
    /// </summary>
    public class SqlUserAccessLoggerConfig : SqlBaseLoggerConfig, ILoggerConfig
    {
        #region Field

        

        #endregion 

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_loggerName">Nome del logger</param>
        /// <param name="_schema">Nome del gestore del database</param>
        /// <param name="_database">Database contenente la tabella di storicizzazione</param>
        /// <param name="_userName">Utente Sql</param>
        /// <param name="_password">Password dell'utente Sql</param>
        /// <param name="_tableName">Nome della tabella di storicizzazione</param>
        /// <param name="_commandToTable">Interfaccia di comunicazione con l'istanza Sql</param>
        public SqlUserAccessLoggerConfig(string _loggerName, string _schema, string _database, string _userName,
            string _password, string _tableName, ICommandToTable _commandToTable)
            : base(_loggerName, _schema, _database, _userName,
                _password, _tableName, _commandToTable)
        {

        }

        #endregion

        #region ILoggerConfig Members

        /// <summary>
        /// Nome del logger.
        /// </summary>
        string ILoggerConfig.Name
        {
            get
            {
                return this.Name;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Crea un'istanza di SqlLogger relativa al LoggerConfig corrente
        /// </summary>
        /// <returns></returns>
        public object Create()
        {
            return new SqlUserAccessLogger(this.Name, this.InitialLevel, this, true); 
        }

        #endregion
    }
}
