// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:36
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Net.Xml
{
    using  WB.Commons.Helpers;

    using WB.IIIParty.Commons.Net.Sockets;

    /// <summary>
    /// Class BaseServerFactory
    /// </summary>
    public abstract class BaseServerFactory : Factory<TcpServer>
    {
        #region Methods

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns>TcpServer.</returns>
        public abstract override TcpServer CreateInstance();

        #endregion Methods
    }
}