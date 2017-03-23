// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:35
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Net.Xml
{
    using  WB.Commons.Helpers;

    using WB.IIIParty.Commons.Net.Sockets;

    /// <summary>
    /// Class BaseClientFactory
    /// </summary>
    public abstract class BaseClientFactory : FactoryWithParams<TcpClient>
    {
        #region Methods

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="parms">The parms.</param>
        /// <returns>TcpClient.</returns>
        public abstract override TcpClient CreateInstance(params object[] parms);

        #endregion Methods
    }
}