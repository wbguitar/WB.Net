// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:14
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Definisce l'interfaccia che restituisce un serializer xml a partire dal tipo di messaggio
    /// </summary>
    public interface IXmlMessageSerializerInfoEx
    {
        #region Methods

        /// <summary>
        /// Ritorna il valore del tag xml specificato
        /// </summary>
        /// <param name="xmlReader">The XML reader.</param>
        /// <param name="xmlTag">The XML tag.</param>
        /// <returns>System.String.</returns>
        string GetSerializeTypeByXmlTag(XmlReader xmlReader, string xmlTag);

        /// <summary>
        /// Ritorna il serializer del messaggio corrispondente
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>XmlSerializer.</returns>
        XmlSerializer GetXmlSerializer(string type);

        #endregion Methods
    }
}