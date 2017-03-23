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
    /// Gestore statico di Logger
    /// </summary>
    public class LoggerManager
    {
        #region Exceptions

        /// <summary>
        /// 
        /// </summary>
        public class DuplicateNameException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg"></param>
            public DuplicateNameException(string msg) : base(msg) { }
        }
        /// <summary>
        /// 
        /// </summary>
        public class InvalidLoggerNameException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg"></param>
            public InvalidLoggerNameException(string msg) : base(msg) { }
        }
        /// <summary>
        /// 
        /// </summary>
        public class InvalidLoggerInterfaceException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg"></param>
            public InvalidLoggerInterfaceException(string msg) : base(msg) { }
        }

        #endregion

        #region Private Fields

        private static Dictionary<string, object> loggers = new Dictionary<string, object>();

        #endregion

        #region Public Static Members

        /// <summary>
        /// Crea il logger configurato
        /// </summary>
        /// <param name="info">Logger</param>
        public static void CreateLogger(ILoggerConfig info)
        {
            if (loggers.ContainsKey(info.Name))
            {
                throw new DuplicateNameException("Logger Name already exist: " + info.Name);
            }
            //Creo l'istanza del logger
            loggers.Add(info.Name, info.Create());
        }
        /// <summary>
        /// Ritorna il logger con il nome specificato che implementa l'interfaccia definita
        /// </summary>
        /// <typeparam name="T">Interfaccia di Log</typeparam>
        /// <param name="name">Nome del Logger</param>
        /// <returns>Logger</returns>
        public static T GetLogger<T>(string name)
        {
            if (!loggers.ContainsKey(name))
            {
                throw new InvalidLoggerNameException("Logger Name not found: " + name);
            }            
            object obj = loggers[name];
            Type t = obj.GetType().GetInterface(typeof(T).FullName);
            if (t == null)
            {
                throw new InvalidLoggerInterfaceException(typeof(T).FullName + " not Implemented on Logger " + name);
            }
            return (T)obj;
        }

        /// <summary>
        /// Distruggo il logger
        /// </summary>
        /// <param name="loggerName"></param>
        public static void DestroyLogger(string loggerName)
        {
            if (!loggers.ContainsKey(loggerName))
            {
                throw new InvalidLoggerNameException("Logger Name not found: " + loggerName);
            }

            object obj = loggers[loggerName];
            Type t = obj.GetType().GetInterface(typeof(IDisposable).FullName);
            if (t != null)
            {
                ((IDisposable)obj).Dispose();
            }
            //Creo l'istanza del logger
            loggers.Remove(loggerName);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Esegue la Dispose su tutti i logger che implementano l'interfaccia IDisposable
        /// </summary>
        public static void Dispose()
        {
            foreach (object obj in loggers.Values)
            {
                Type t = obj.GetType().GetInterface(typeof(IDisposable).FullName);
                if (t != null)
                {
                    ((IDisposable)obj).Dispose();
                }
            }
        }

        #endregion
    }
}
