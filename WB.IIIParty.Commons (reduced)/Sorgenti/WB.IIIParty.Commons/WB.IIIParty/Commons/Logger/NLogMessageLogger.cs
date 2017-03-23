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
    /// Implementa un Logger su un file con interfaccia NLog
    /// </summary>
    public class NLogMessageLogger : NLogLogger, IMessageLog
    {
        #region Field

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_config">Configurazione</param>
        public NLogMessageLogger(NLogLoggerConfig _config)
            : base(_config)
        {
         
        }

        #endregion
  
        #region Property       

        #endregion


        #region ITraceLog Members

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
            if (!CanLog(level))
            {
                return;
            }

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

            string nm = string.Empty;
            string sn = string.Empty;

            if (caller != null)
            {
                nm=caller.GetType().Namespace;
                sn = caller.GetType().Name;
            }


            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(level), base.Config.NLogTargetName, message);

            entryLog.Properties["SenderNamespace"] = nm;
            entryLog.Properties["SenderName"] = sn;
            entryLog.Properties["MethodName"] = methodName;
            entryLog.Properties["TetLoggerName"] = base.Config.Name;
            
            base.Logger.Log(entryLog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevels level, string message)
        {
            Log(level, null, message);
        }

        /// <summary>
        /// Livello di log
        /// </summary>
        public LogLevels LogLevel
        {
            get
            {
                return base.GetTetlogLevel(base.GetCurrentNLogLevel());                
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}