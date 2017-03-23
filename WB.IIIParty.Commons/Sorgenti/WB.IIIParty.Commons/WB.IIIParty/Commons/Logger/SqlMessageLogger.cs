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
using WB.IIIParty.Commons.Data;

using System.Diagnostics;
using System.Reflection;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa un Logger su un database SqlServer con interfaccia ILog
    /// </summary>
    public class SqlMessageLogger : SqlBaseLogger, IMessageLog
    {
        #region Field

        private List<object> valueTable = new List<object>();

        private int randomNumber = 0;
        Random randomNumberObject = new Random(); 
        
        private Process myProcess;
        private readonly int processID;

        #endregion 

        #region Constructor
        /// <summary>
        /// Costruttore SqlTraceLogger
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        /// <param name="_initialLevel">Livello iniziale del logger</param>
        /// <param name="_config">SqlMessageLoggerConfig</param>
        /// <param name="_activeLogLevelFromRegistry">Abilita la possibilità di modificare il livello di log da registro</param>
        public SqlMessageLogger(string _name, LogLevels _initialLevel, SqlMessageLoggerConfig _config, bool _activeLogLevelFromRegistry)
            : base(_name, _initialLevel, _config, _activeLogLevelFromRegistry)
        {
            this.myProcess = Process.GetCurrentProcess();
            this.processID = this.myProcess.Id;

            this.columnsNameList.Add("log_Ts");
            this.columnsNameList.Add("log_ProcessID");
            this.columnsNameList.Add("log_IncrementalNumber");
            this.columnsNameList.Add("log_level");
            this.columnsNameList.Add("log_ThreadID");
            this.columnsNameList.Add("log_SenderNamespace");
            this.columnsNameList.Add("log_SenderName");
            this.columnsNameList.Add("log_MethodName");
            this.columnsNameList.Add("log_Message");
        }

        #endregion
                
        #region IMessageLog Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public void Log(LogLevels level, object caller, string message)
        {
            lock (this.thisLock)
            {
                if (!CanLog(level))
                {
                    return;
                }

                this.valueTable = new List<object>();

                //Called Method
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase method = stackFrame.GetMethod();
                string methodName = method.Name;
                if (methodName.Equals("Log"))
                {
                    stackFrame = stackTrace.GetFrame(2);
                    method = stackFrame.GetMethod();
                    methodName = method.Name;
                }

                this.valueTable.Add(DateTime.Now);
                this.valueTable.Add(this.processID);
                this.randomNumber++;
                this.valueTable.Add(this.randomNumber);
                this.valueTable.Add(level.ToString());
                this.valueTable.Add(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                this.valueTable.Add(caller.GetType().Namespace);
                this.valueTable.Add(caller.GetType().Name);
                this.valueTable.Add(methodName);
                if (level == LogLevels.Disabled)
                {
                    this.valueTable.Add("No message. LogLevel = Disable");
                }
                else
                {
                    this.valueTable.Add(message);
                }

                base.Log(this.valueTable);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevels level, string message)
        {
            lock (this.thisLock)
            {
                if (!CanLog(level))
                {
                    return;
                }

                this.valueTable = new List<object>();

                //Called Method
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase method = stackFrame.GetMethod();
                string methodName = method.Name;
                if (methodName.Equals("Log"))
                {
                    stackFrame = stackTrace.GetFrame(2);
                    method = stackFrame.GetMethod();
                    methodName = method.Name;
                }

                this.valueTable.Add(DateTime.Now);
                this.valueTable.Add(this.processID);
                this.randomNumber++;
                this.valueTable.Add(this.randomNumber);
                this.valueTable.Add(level.ToString());
                this.valueTable.Add(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                this.valueTable.Add("");
                this.valueTable.Add("");
                this.valueTable.Add("");
                if (level == LogLevels.Disabled)
                {
                    this.valueTable.Add("No message. LogLevel = Disable");
                }
                else
                {
                    this.valueTable.Add(message);
                }

                base.Log(this.valueTable);
            }
        }
        /// <summary>
        /// Livello di log.
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
        /// 
        /// </summary>
        /// <param name="level"></param>
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
    }
}
