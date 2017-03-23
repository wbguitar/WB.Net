// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:19
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Forms
{
    using System;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// Struct Log
    /// </summary>
    public struct Log
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> struct.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="parms">The parms.</param>
        public Log(DateTime timeStamp, LogLevels logLevel, string msg, params object[] parms)
            : this()
        {
            TimeStamp = timeStamp;
            LogLevel = logLevel;
            Text = string.Format(msg, parms);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevels LogLevel
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp
        {
            get; internal set;
        }

        #endregion Properties
    }
}