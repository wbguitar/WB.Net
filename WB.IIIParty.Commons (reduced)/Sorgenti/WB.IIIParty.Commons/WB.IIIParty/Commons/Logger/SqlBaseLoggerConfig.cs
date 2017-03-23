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
    /// Oggetto base per la creazione dei logger config.
    /// </summary>
    public abstract class SqlBaseLoggerConfig : LoggerConfig
    {
        #region Field

        private string database;
        private string userName;
        private string password;
        private string tableName;
        private string schema;
        private ICommandToTable commandToTable;

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
        public SqlBaseLoggerConfig(string _loggerName, string _schema, string _database, string _userName,
            string _password, string _tableName,ICommandToTable _commandToTable)
            : base(_loggerName, LogLevels.Debug)
        {
            this.commandToTable = _commandToTable;
            this.database = _database;
            this.userName = _userName;
            this.password = _password;
            this.tableName = _tableName;
            this.schema = _schema;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Ritorna l'interfaccia di comunicazione con l'istanza Sql
        /// </summary>
        public ICommandToTable CommandToTable
        {
            get
            {
                return this.commandToTable;
            }
        }

        /// <summary>
        /// Nome del gestore del database
        /// </summary>
        public string Schema
        {
            get
            {
                return this.schema;
            }
        }

        /// <summary>
        /// Ritorna il nome del Database corrente
        /// </summary>
        public string Database
        {
            get
            {
                return this.database;
            }
        }

        /// <summary>
        /// Ritorna il nome utente Sql
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }
        }

        /// <summary>
        /// Ritorna la password Sql
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }
        }

        /// <summary>
        /// Ritorna il nome della tabella di Log
        /// </summary>
        public string TableName
        {
            get
            {
                return this.tableName;
            }
        }

        #endregion
    }
}
