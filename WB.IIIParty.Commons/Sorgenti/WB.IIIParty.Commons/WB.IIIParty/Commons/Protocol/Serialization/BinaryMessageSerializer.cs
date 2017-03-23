// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2010
//Progetto: AMIL5
//Autore: Acquisti Leonard
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Protocol.Serialization
{
    /// <summary>
    /// Implemena l'interfaccia IMessageParser per la serializzazione binaria di messaggi su uno stream
    /// </summary>
    public class BinaryMessageSerializer : IMessageParser
    {
        #region Private Variables

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        #endregion

        #region IMessageParser Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CanReadLength(byte[] data)
        {            
            if (data.Length >= 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetLength(byte[] data)
        {
            short lenght = BitConverter.ToInt16(data, 0);
            return lenght;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IMessage ParseMessage(byte[] data)
        {            
            object obj = null;
            System.IO.MemoryStream ms =new System.IO.MemoryStream(data);
            ms.Position = 2;
            obj = binaryFormatter.Deserialize(ms);
            ms.Dispose();
            if (obj is IMessage) return (IMessage)obj;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] SerializeMessage(IMessage data)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            binaryFormatter.Serialize(ms,data);
            byte[] body = ms.ToArray();
            byte[] header = new byte[2];
            Array.Copy(BitConverter.GetBytes((short)body.Length + 2), 0, header, 0, 2);
            byte[] message = new byte[header.Length + body.Length];
            Array.Copy(header, 0, message, 0, header.Length);
            Array.Copy(body, 0, message, header.Length, body.Length);
            ms.Dispose();
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SerializeIsSupported()
        {
            return true;
        }

        #endregion
    }
}
