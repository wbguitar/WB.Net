// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:23
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Net.Xml
{
    using WB.IIIParty.Commons.Net.Sockets;

    /// <summary>
    /// Interface IClientConfig
    /// </summary>
    public interface IClientConfig
    {
        #region Methods

        /// <summary>
        /// Creates the TCP client.
        /// </summary>
        /// <returns>TcpClient.</returns>
        TcpClient CreateTcpClient();

        #endregion Methods
    }
}