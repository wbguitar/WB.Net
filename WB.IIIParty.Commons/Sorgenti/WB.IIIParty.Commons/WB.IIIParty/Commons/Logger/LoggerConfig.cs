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

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa la classe base di configurazione di un Logger
    /// </summary>
    public abstract class LoggerConfig
    {
        #region Private Field

        private string name;
        private LogLevels initialLevel;

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        /// <param name="_initialLevel">Filtro iniziale del livello di log</param>
        public LoggerConfig(string _name, LogLevels _initialLevel)
        {
            this.name = _name;
            this.initialLevel = _initialLevel;
            //throw new NotImplementedException();
        }

        #endregion

        #region Members

        #endregion

        #region Properties

        /// <summary>
        /// Ritorna il nome del logger
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        /// <summary>
        /// Ritorna il livello di filtro iniziale di Log
        /// </summary>
        public LogLevels InitialLevel
        {
            get
            {
                return this.initialLevel;
            }
        }

        #endregion
    }
}
