using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Protocol.Serialization.Exceptions
{
    /// <summary>
    /// Eccezione generata quando non viene trovato un serializer valido per il messaggio XML
    /// </summary>
    public class SerializerNotFoundException : WB.IIIParty.Commons.Protocol.MessageParseException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SerializerNotFoundException(string message)
            : base(message)
        {
        }
    }
}
