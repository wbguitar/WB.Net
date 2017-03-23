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
    using WB.IIIParty.Commons.Protocol;

    /// <summary>
    /// Class BaseRedundantClientFactory
    /// </summary>
    public abstract class BaseRedundantClientFactory : FactoryWithParams<RedundantClientTcp>
    {
        #region Methods

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="parms">The parms.</param>
        /// <returns>RedundantClientTcp.</returns>
        public abstract override RedundantClientTcp CreateInstance(params object[] parms);

        #endregion Methods
    }
}