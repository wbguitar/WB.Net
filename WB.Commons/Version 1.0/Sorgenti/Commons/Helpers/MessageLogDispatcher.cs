// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:17
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    //using System.Threading.Tasks;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// Class MessageLogDispatcher
    /// </summary>
    public class MessageLogDispatcher : IMessageLog
    {
        private static MessageLogDispatcher instance;
        public static MessageLogDispatcher Instance
        {
            get
            {
                if (instance == null)
                    instance = new MessageLogDispatcher();

                return instance;
            }
        }


        #region Events
        /// <summary>
        /// Occurs when [on log1].
        /// </summary>
        private event dLog1 onLog1 = (ll, s) => { };
        /// <summary>
        /// Occurs when [on log2].
        /// </summary>
        private event dLog2 onLog2 = (ll, o, s) => { };
        #endregion

        #region Delegates

        /// <summary>
        /// Delegate dLog1
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        private delegate void dLog1(LogLevels level, string message);

        /// <summary>
        /// Delegate dLog2
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="message">The message.</param>
        private delegate void dLog2(LogLevels level, object caller, string message);

        #endregion Delegates

        #region Properties

        /// <summary>
        /// Imposta o Ritorna il livello di Log corrente
        /// </summary>
        /// <value>The log level.</value>
        public LogLevels LogLevel
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ritorna se il livello di log specificato è abilitato
        /// </summary>
        /// <param name="level">Livello di Log</param>
        /// <returns><c>true</c> if this instance can log the specified level; otherwise, <c>false</c>.</returns>
        public bool CanLog(LogLevels level)
        {
            return true;
        }

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="message">Messaggio di log</param>
        public void Log(LogLevels level, string message)
        {
            LogLevel = level;
            onLog1(level, message);
            
        }

        

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante la funzione di log</param>
        /// <param name="message">Messaggio di log</param>
        public void Log(LogLevels level, object caller, string message)
        {
            LogLevel = level;
            onLog2(level, caller, message);
            
        }

        /// <summary>
        /// Registers the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Register(IMessageLog logger)
        {
            onLog1 += logger.Log;
            onLog2 += logger.Log;
        }

        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void UnRegister(IMessageLog logger)
        {
            onLog1 -= logger.Log;
            onLog2 -= logger.Log;
        }

        #endregion Methods
    }
}