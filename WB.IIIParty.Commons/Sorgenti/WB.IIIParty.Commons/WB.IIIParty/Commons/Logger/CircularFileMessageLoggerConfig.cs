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

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa un LoggerConfig per il logger CircularFileLogger
    /// </summary>
    public class CircularFileMessageLoggerConfig : LoggerConfig , ILoggerConfig
    {
        #region Field

        private string loggerName;
        private LogLevels initialLevel;
        private string fileName;
        private int fileSize;

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_loggerName">Nome del logger</param>
        /// <param name="_initialLevel">Livello di filtro dei messaggi di Log</param>
        /// <param name="_fileName">Nome del file di Log</param>
        /// <param name="_fileSize">Dimensione massima del file di Log</param>
        public CircularFileMessageLoggerConfig(string _loggerName, LogLevels _initialLevel, string _fileName, int _fileSize)
            : base(_loggerName, _initialLevel)
        {
            this.loggerName = _loggerName;
            this.initialLevel = _initialLevel;
            this.fileName = _fileName;
            this.fileSize = _fileSize;
        }

        #endregion

        #region Property

        /// <summary>
        /// Ritorna il path e il nome del file di log
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        /// <summary>
        /// Ritorna la dimensione max per i file di log
        /// </summary>
        public int FileSize
        {
            get
            {
                return this.fileSize;
            }
        }

        #endregion

        #region  Members

        /// <summary>
        /// Crea un'istanza di FileLogger relativa al LoggerConfig corrente
        /// </summary>
        /// <returns></returns>
        public object Create()
        {
            return new CircularFileMessageLogger(this);
        }
        #endregion
    }
}
