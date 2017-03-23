using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WB.IIIParty.Commons.Collections;
using System.Threading;
using WB.IIIParty.Commons.Logger;

 
namespace WB.IIIParty.Commons.Sql
{
    /// <summary>
    /// Log su fle di testo
    /// </summary>
    public class LoggerToFile : IDisposable
    {
        #region Static

        static private int referenceCounter = 0;
                        
        #endregion Static

        #region PrivateFiled

        private const string LoggerName = "WB.IIIParty.Commons.Data.Sql.LoggerToFile";
        private const string FileName = "TetSistemiCommonsDataSqlDbSync.txt";
        private const int FileSize = 2097152; //2 MB
        static private LoggerToFile loggerToFile_singleton = null;
                
        #endregion PrivateFiled

        #region PublicField

        #endregion PublicField

        #region Property

        static object synchObj = new object();
        /// <summary>
        /// Restituisce un puntatore al file di log
        /// </summary>
        static public LoggerToFile LoggerToFile_singleton
        {
            get
            {
                lock (synchObj)
                {
                    if (loggerToFile_singleton == null)
                    {
                        loggerToFile_singleton = new LoggerToFile();
                    }

                    return loggerToFile_singleton;
                }
            }

        }

        #endregion Property

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        public LoggerToFile()
        {
            
        }

        #endregion Constructor
        
        #region PublicMethod

        /// <summary>
        /// Restiutisce il logger
        /// </summary>
        /// <returns></returns>
        public IMessageLog GetLogger()
        {
            lock (this)
            {
                if (referenceCounter <= 0)
                {
                    CircularFileMessageLoggerConfig logger = new CircularFileMessageLoggerConfig(LoggerName,
                        LogLevels.Debug, FileName, FileSize);
                    LoggerManager.CreateLogger(logger);
                    referenceCounter = 0;
                }
                referenceCounter++;
                return LoggerManager.GetLogger<IMessageLog>(LoggerName);
            }
        }

        

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                referenceCounter--;
                if (referenceCounter == 0)
                    LoggerManager.DestroyLogger(LoggerName);
                //if (referenceCounter <0)
                //    referenceCounter = 0;
            }
        }

        #endregion PublicMethod
    }
}


