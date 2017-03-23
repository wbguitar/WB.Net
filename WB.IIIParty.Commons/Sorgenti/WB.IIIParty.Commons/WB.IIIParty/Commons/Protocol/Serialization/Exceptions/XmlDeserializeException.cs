using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Protocol.Serialization.Exceptions
{
    /// <summary>
    /// Eccezione generata quando ci sono errori di deserializazione del messaggio XML
    /// </summary>
    public class XmlDeserializeException : WB.IIIParty.Commons.Protocol.MessageParseException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public XmlDeserializeException(string message)
            : base(message)
        {
        }
    }
}
