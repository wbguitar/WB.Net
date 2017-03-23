using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Protocol.Serialization.Exceptions
{
    /// <summary>
    /// Eccezione generata quando ci sono errori di serializazione del messaggio XML
    /// </summary>
    public class XmlSerializeException : WB.IIIParty.Commons.Protocol.MessageParseException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public XmlSerializeException(string message)
            : base(message)
        {
        }
    }
}
