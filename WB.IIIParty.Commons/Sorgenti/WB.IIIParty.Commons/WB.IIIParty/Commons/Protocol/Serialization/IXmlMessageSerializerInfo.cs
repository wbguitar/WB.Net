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
using System.Xml.Serialization;

namespace WB.IIIParty.Commons.Protocol.Serialization
{
    /// <summary>
    /// Definisce l'interfaccia che restituisce un serializer xml a partire dal tipo di messaggio
    /// </summary>
    public interface IXmlMessageSerializerInfo
    {
        /// <summary>
        /// Ritorna il serializer del messaggio corrispondente
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        XmlSerializer GetXmlSerializer(string type);
    }
}
