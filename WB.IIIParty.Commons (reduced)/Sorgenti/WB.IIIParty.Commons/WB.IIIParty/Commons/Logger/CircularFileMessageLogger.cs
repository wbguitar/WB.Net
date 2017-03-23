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

using System.IO;

using System.Diagnostics;
using System.Reflection;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa un Logger su un file con interfaccia ILog
    /// </summary>
    public class CircularFileMessageLogger : IMessageLog, IDisposable
    {
        #region Field

        private CircularFileMessageLoggerConfig fileConfig;
        private LogLevels logLevelFilter;

        private StreamWriter fsStreamW;
        private FileInfo fsInfo;

        private string fileExt = ".txt";
        private string fileName = @".\Log";
        private string fileSuffisso = "_A";
        private int dimensioneMaxFile = 524288; // 512 * 1024(1 Kb)

        private Object thisLock = new Object();

        private bool fileAorB = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="config">Configurazione</param>
        public CircularFileMessageLogger(CircularFileMessageLoggerConfig config)
        {            
            try
            {
                lock (this.thisLock)
                {
                    this.fileConfig = config;
                    this.dimensioneMaxFile = this.fileConfig.FileSize;
                    this.logLevelFilter = config.InitialLevel;
                    //divido il nome del file dal estensione per inserire il suffisso
                    int posEstensione = this.fileConfig.FileName.LastIndexOf('.');
                    this.fileName = this.fileConfig.FileName.Substring(0, posEstensione);
                    this.fileExt = this.fileConfig.FileName.Substring(posEstensione, (this.fileConfig.FileName.Length - posEstensione));


                    FileInfo fsInfoA = new FileInfo(this.fileName + "_A" + this.fileExt);
                    FileInfo fsInfoB = new FileInfo(this.fileName + "_B" + this.fileExt);
                    if ((fsInfoA.Exists) && (fsInfoB.Exists))
                    {
                        if (fsInfoA.LastWriteTime > fsInfoB.LastWriteTime)
                        {
                            this.fileSuffisso = "_A";
                        }
                        else
                        {
                            this.fileSuffisso = "_B";
                        }
                    }
                    else
                    {
                        this.fileSuffisso = "_A";
                        if (fsInfoB.Exists)
                        {
                            this.fileSuffisso = "_B";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region IMessageLog Members

        ///<summary>
        /// Inserisce un nuovo messaggio di Log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante</param>
        /// <param name="message">Messaggio di Log</param>
        public void Log(LogLevels level, object caller, string message)
        {
            lock (this.thisLock)
            {
                if (!CanLog(level))
                {
                    return;
                }
                try
                {
                    System.Threading.Thread threadExecute = System.Threading.Thread.CurrentThread;

                    //Called Method
                    StackTrace stackTrace = new StackTrace();
                    StackFrame stackFrame = stackTrace.GetFrame(1);
                    MethodBase method = stackFrame.GetMethod();
                    string methodName = method.Name;

                    string messagePrint = DateTime.Now.ToString() + " - ";
                    messagePrint += level.ToString() + " - ";
                    messagePrint += threadExecute.ManagedThreadId.ToString() + " - ";
                    messagePrint += caller.GetType().Namespace + ".";
                    messagePrint += caller.GetType().Name + ".";
                    messagePrint += methodName + " - ";
                    if (level == LogLevels.Disabled)
                    {
                        messagePrint += "No message. LogLevel = Disable";
                    }
                    else
                    {
                        messagePrint += message;
                    }

                    PrintLog(messagePrint, level);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Inserisce un nuovo messaggio di Log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="message">Messaggio di Log</param>
        public void Log(LogLevels level, string message)
        {
            lock (this.thisLock)
            {
                if (!CanLog(level))
                {
                    return;
                }
                try
                {
                    System.Threading.Thread threadExecute = System.Threading.Thread.CurrentThread;

                    //Called Method
                    StackTrace stackTrace = new StackTrace();
                    StackFrame stackFrame = stackTrace.GetFrame(1);
                    MethodBase method = stackFrame.GetMethod();
                    string methodName = method.Name;

                    string messagePrint = DateTime.Now.ToString() + " - ";
                    messagePrint += level.ToString() + " - ";
                    messagePrint += threadExecute.ManagedThreadId.ToString() + " - ";
                    if (level == LogLevels.Disabled)
                    {
                        messagePrint += "No message. LogLevel = Disable";
                    }
                    else
                    {
                        messagePrint += message;
                    }
                    

                    PrintLog(messagePrint, level);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Stampo il messaggio di Log completo, sul file corretto (_A o _B).
        /// </summary>
        /// <param name="_messagePrint">Messaggio di Log</param>
        /// <param name="_level">Livello del log</param>
        private void PrintLog(string _messagePrint, LogLevels _level)
        {
            if (!CanLog(_level))
            {
                return;
            }

            lock (this.thisLock)
            {
                this.fsInfo = new FileInfo(this.fileName + this.fileSuffisso + this.fileExt);

                if (this.fsInfo.Exists)
                {
                    if (this.fsInfo.Length > (dimensioneMaxFile / 2))
                    {
                        if (this.fileAorB)
                        {
                            this.fileAorB = false;
                            this.fileSuffisso = "_A";
                            this.fsInfo = new FileInfo(this.fileName + this.fileSuffisso + this.fileExt);
                            if (this.fsInfo.Length > (dimensioneMaxFile / 2))
                            {
                                this.fsInfo.Delete();
                            }
                        }
                        else
                        {
                            this.fileAorB = true;
                            this.fileSuffisso = "_B";
                            this.fsInfo = new FileInfo(this.fileName + this.fileSuffisso + this.fileExt);
                            if (this.fsInfo.Length > (dimensioneMaxFile / 2))
                            {
                                this.fsInfo.Delete();
                            }
                        }
                    }
                }
                this.fsStreamW = File.AppendText(this.fileName + this.fileSuffisso + this.fileExt);

                this.fsStreamW.WriteLine(_messagePrint);

                this.fsStreamW.Close();
            }
        }

        #endregion
            
        #region Property

        /// <summary>
        /// Imposta o Ritorna il filtro del livello di log
        /// </summary>
        public LogLevels LogLevel
        {
            get
            {
                return this.logLevelFilter;
            }
            set
            {
                this.logLevelFilter = value;
            }
        }

        /// <summary>
        /// Ritorna se il livello di log specificato è abilitato
        /// </summary>
        /// <param name="level">Livello di Log</param>
        /// <returns></returns>
        public bool CanLog(LogLevels level)
        {
            bool canLog = false;
            if (this.logLevelFilter >= level)
            {
                canLog = true;
            }

            return canLog;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Libera le risorse
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this.fsStreamW != null)
                {
                    this.fsStreamW.Dispose();
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}