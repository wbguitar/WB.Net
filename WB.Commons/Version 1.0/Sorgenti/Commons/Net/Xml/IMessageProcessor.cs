// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:23
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Net.Xml
{
    using WB.IIIParty.Commons.Protocol;

    /// <summary>
    /// Interface IMessageProcessor
    /// </summary>
    public interface IMessageProcessor
    {
        #region Methods

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>IMessage.</returns>
        IMessage ProcessMessage(IMessage msg);

        #endregion Methods
    }
}