// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:12
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

using WB.IIIParty.Commons.Logger;

namespace WB.Commons.Net
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using WB.IIIParty.Commons.Protocol;

    #region Delegates

    /// <summary>
    /// Delegate dLogAction
    /// </summary>
    /// <param name="msg">The MSG.</param>
    /// <param name="parms">The parms.</param>
    public delegate void dLogAction(string msg, params object[] parms);

    #endregion Delegates

    /// <summary>
    /// Interface IClient
    /// </summary>
    public interface IClient
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="imsg">The imsg.</param>
        void SendMessage(IMessage imsg);

        /// <summary>
        /// Sends the message async.
        /// </summary>
        /// <param name="imsg">The imsg.</param>
        /// <param name="cback">The cback.</param>
        /// <param name="parm">The parm.</param>
        /// <returns>ASyncResult.</returns>
        ASyncResult SendMessageAsync(IMessage imsg, dAsyncCallback cback, object parm);

        /// <summary>
        /// Sends the message sync.
        /// </summary>
        /// <param name="imsg">The imsg.</param>
        /// <param name="msgout">The msgout.</param>
        /// <param name="vEx">The v ex.</param>
        /// <returns>SyncResult.</returns>
        SyncResult SendMessageSync(IMessage imsg, out List<IMessage> msgout, out List<MessageValidationException> vEx);

        #endregion Methods
    }
}