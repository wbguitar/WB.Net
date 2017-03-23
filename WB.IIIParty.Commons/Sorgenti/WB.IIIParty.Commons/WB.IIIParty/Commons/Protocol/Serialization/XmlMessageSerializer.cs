// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2010
//Progetto: AMIL5
//Autore: Acquisti Leonard
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2012-07-10 15:08:24 +0200 (mar, 10 lug 2012) $
//Versione: $Rev: 107 $
// ------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WB.IIIParty.Commons.Protocol.Serialization.Exceptions;

namespace WB.IIIParty.Commons.Protocol.Serialization
{
    /// <summary>
    /// Implemena l'interfaccia IMessageParser per la serializzazione xml di messaggi su uno stream
    /// </summary>
    public class XmlMessageSerializer : IMessageParser
    {
        #region Const

        private const int HeaderLength = 4;

        #endregion

        #region Private Variables

        IXmlMessageSerializerInfo xmlMessageSerializerInfo;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_xmlMessageSerializerInfo"></param>
        public XmlMessageSerializer(IXmlMessageSerializerInfo _xmlMessageSerializerInfo)
        {
            xmlMessageSerializerInfo = _xmlMessageSerializerInfo;
        }

        #endregion

        #region IMessageParser Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CanReadLength(byte[] data)
        {            
            //servono 4 bytes x la lunghezza del messaggio
            if (data.Length >= HeaderLength)
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
            //lunghezza del messaggio
            int lenght = BitConverter.ToInt32(data, 0);
            return lenght;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IMessage ParseMessage(byte[] data)
        {
            //estraggo solo l'array xml
            int lenght = BitConverter.ToInt32(data, 0) - HeaderLength;
            byte[] body = new byte[lenght];
            Array.Copy(data, HeaderLength, body, 0, lenght);
            //creo un memorystream
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(body))
            {
                string root = string.Empty;
                //estraggo il nome del primo nodo / tipo di messaggio
                using (XmlReader reader = XmlReader.Create(ms))
                {
                    while (reader.Read())
                    {
                        // first element is the root element 
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            root = reader.Name;
                            break;
                        }
                    }
                }
                //ricavo il serializer
                XmlSerializer serializer = xmlMessageSerializerInfo.GetXmlSerializer(root);
                if (serializer == null)
                {
                    throw new SerializerNotFoundException("Serializer not found: " + root);
                }

                //deserializzo l'XML
                object obj = null;
                ms.Position = 0;
                try
                {
                    obj = serializer.Deserialize(ms);
                }
                catch (Exception ex)
                {
                    throw new XmlDeserializeException("Xml Deserialize Exception: " + ex.Message);

                }

                if (obj is IMessage) return (IMessage)obj;
                else return null;
            }                       
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SerializeMessage(IMessage data,string filename)
        {
            //rimuovo il namespace dall'xml
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            //ricavo il serializer
            string serializerName = data.GetType().Name;
            XmlSerializer serializer = xmlMessageSerializerInfo.GetXmlSerializer(serializerName);
            if (serializer == null)
            {
                throw new SerializerNotFoundException("Serializer not found: " + serializerName);
            }

            System.Xml.XmlWriter w = System.Xml.XmlWriter.Create(filename);

            try
            {
                serializer.Serialize(w, data, namespaces);

            }
            catch (Exception ex)
            {
                throw new XmlSerializeException("Xml Serialize Exception: " + ex.Message);
            }

            w.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] SerializeMessage(IMessage data)
        {
            //rimuovo il namespace dall'xml
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            //ricavo il serializer
            string serializerName = data.GetType().Name;
            XmlSerializer serializer = xmlMessageSerializerInfo.GetXmlSerializer(serializerName);
            if (serializer == null)
            {
                throw new SerializerNotFoundException("Serializer not found: " + serializerName);
            }

            

            //serializzo il messaggio
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                try
                {
                    serializer.Serialize(ms, data, namespaces);

                }
                catch (Exception ex)
                {
                    throw new XmlSerializeException("Xml Serialize Exception: " + ex.Message);
                }

                //aggiungo l'header
                byte[] body = ms.ToArray();
                byte[] header = new byte[4];
                Array.Copy(BitConverter.GetBytes((int)body.Length + 4), 0, header, 0, 4);

                //aggiungo l'xml
                byte[] message = new byte[header.Length + body.Length];
                Array.Copy(header, 0, message, 0, header.Length);
                Array.Copy(body, 0, message, header.Length, body.Length);
                return message;
            }
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
