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
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using WB.IIIParty.Commons.Protocol;
    using WB.IIIParty.Commons.Protocol.Serialization;
    using WB.IIIParty.Commons.Protocol.Serialization.Exceptions;

    /// <summary>
    /// Implemena l'interfaccia IMessageParser per la serializzazione xml di messaggi su uno stream
    /// </summary>
    public class XmlMessageSerializerEx : IMessageParser
    {
        #region Fields

        /// <summary>
        /// The header length
        /// </summary>
        private const int HeaderLength = 4;

        /// <summary>
        /// The XML message serializer info ex
        /// </summary>
        IXmlMessageSerializerInfoEx xmlMessageSerializerInfoEx;

        /// <summary>
        /// The XML tag
        /// </summary>
        string xmlTag;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMessageSerializerEx"/> class.
        /// </summary>
        /// <param name="_xmlMessageSerializerInfoEx">The _XML message serializer info ex.</param>
        public XmlMessageSerializerEx(IXmlMessageSerializerInfoEx _xmlMessageSerializerInfoEx)
        {
            this.xmlMessageSerializerInfoEx = _xmlMessageSerializerInfoEx;
            this.xmlTag = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMessageSerializerEx"/> class.
        /// </summary>
        /// <param name="_xmlMessageSerializerInfoEx">The _XML message serializer info ex.</param>
        /// <param name="_xmlTag">The _XML tag.</param>
        public XmlMessageSerializerEx(IXmlMessageSerializerInfoEx _xmlMessageSerializerInfoEx, string _xmlTag)
        {
            this.xmlMessageSerializerInfoEx = _xmlMessageSerializerInfoEx;
            this.xmlTag = _xmlTag;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Ritorna se dai dati a disposizione è possibile calcolare la lunghezza COMPLESSIVA del messaggio
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if this instance [can read length] the specified data; otherwise, <c>false</c>.</returns>
        public bool CanReadLength(byte[] data)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            //ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            //string xmlMessage = asciiEncoding.GetString(data);
            string xmlMessage = utf8Encoding.GetString(data);
            if (xmlMessage.Contains("</Ihm>"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ritorna la lunghezza COMPLESSIVA del messaggio
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Int32.</returns>
        public int GetLength(byte[] data)
        {
            //lunghezza del messaggio
            //ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            //string xmlMessage = asciiEncoding.GetString(data);
            string xmlMessage = utf8Encoding.GetString(data);
            int index = xmlMessage.IndexOf("</Ihm>");
            int lenght = index + "</Ihm>".Count();
            return lenght;
        }

        /// <summary>
        /// Effettua la deserializzazione di un messaggio da un array di byte
        /// Errori di parsing devono essere generati ereditando da MessageParseException
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>IMessage.</returns>
        /// <exception cref="WB.IIIParty.Commons.Protocol.Serialization.Exceptions.SerializerNotFoundException">Serializer not found:  + serializeType</exception>
        /// <exception cref="WB.IIIParty.Commons.Protocol.Serialization.Exceptions.XmlDeserializeException">Xml Deserialize Exception:  + ex.Message</exception>
        public IMessage ParseMessage(byte[] data)
        {
            //estraggo solo l'array xml
            //int lenght = BitConverter.ToInt32(data, 0) - HeaderLength;
            //byte[] body = new byte[lenght];
            //Array.Copy(data, HeaderLength, body, 0, lenght);
            //creo un memorystream
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
            {
                string serializeType = string.Empty;
                //estraggo il nome del primo nodo / tipo di messaggio
                using (XmlReader reader = XmlReader.Create(ms))
                {
                    if (this.xmlTag == string.Empty)
                    {
                        while (reader.Read())
                        {
                            // first element is the root element
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                serializeType = reader.Name;
                                break;
                            }
                        }
                    }
                    else
                    {
                        serializeType = xmlMessageSerializerInfoEx.GetSerializeTypeByXmlTag(reader, this.xmlTag);
                    }
                }
                //ricavo il serializer
                XmlSerializer serializer = xmlMessageSerializerInfoEx.GetXmlSerializer(serializeType);
                if (serializer == null)
                {
                    throw new SerializerNotFoundException("Serializer not found: " + serializeType);
                }

                //deserializzo l'XML
                object obj = null;
                ms.Position = 0;
                try
                {
                    using (StreamReader streamReader = new StreamReader(ms, System.Text.Encoding.UTF8))
                    {
                        obj = serializer.Deserialize(streamReader);
                    }

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
        /// Ritorna se la classe può serializzare un oggetto
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool SerializeIsSupported()
        {
            return true;
        }

        /// <summary>
        /// Serializes the message.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="filename">The filename.</param>
        /// <exception cref="WB.IIIParty.Commons.Protocol.Serialization.Exceptions.SerializerNotFoundException">Serializer not found:  + serializerName</exception>
        /// <exception cref="WB.IIIParty.Commons.Protocol.Serialization.Exceptions.XmlSerializeException">Xml Serialize Exception:  + ex.Message</exception>
        public void SerializeMessage(IMessage data, string filename)
        {
            //rimuovo il namespace dall'xml
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            //ricavo il serializer
            string serializerName = data.GetType().Name;
            XmlSerializer serializer = xmlMessageSerializerInfoEx.GetXmlSerializer(serializerName);
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
        /// Effettua la serializzazione di un messaggio in un array di byte
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Byte[][].</returns>
        /// <exception cref="WB.IIIParty.Commons.Protocol.Serialization.Exceptions.SerializerNotFoundException">Serializer not found:  + serializerName</exception>
        /// <exception cref="WB.IIIParty.Commons.Protocol.Serialization.Exceptions.XmlSerializeException">Xml Serialize Exception:  + ex.Message</exception>
        public byte[] SerializeMessage(IMessage data)
        {
            byte[] message = null;
            //rimuovo il namespace dall'xml
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            //ricavo il serializer
            string serializerName = data.GetType().Name;
            XmlSerializer serializer = xmlMessageSerializerInfoEx.GetXmlSerializer(serializerName);
            if (serializer == null)
            {

                throw new SerializerNotFoundException("Serializer not found: " + serializerName);
            }
            //serializzo il messaggio
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.Indent = false;
                xmlWriterSettings.NewLineOnAttributes = false;
                xmlWriterSettings.NewLineHandling = NewLineHandling.None;
                xmlWriterSettings.OmitXmlDeclaration = false;
                xmlWriterSettings.NamespaceHandling = NamespaceHandling.Default;
                using ( XmlWriter xmlWriter = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    try
                    {
                        serializer.Serialize(xmlWriter, data, namespaces);
                    }
                    catch (Exception ex)
                    {
                        throw new XmlSerializeException("Xml Serialize Exception: " + ex.Message);
                    }
                    message = ms.ToArray();
                    //byte[] body = ms.ToArray();
                    //**************** ORRORE/SPAVENTO ******************
                    //message = new byte[body.Length - 3];
                    //Array.Copy(body, 3, message, 0, message.Length);
                    //***************************************************
                }
                return message;
            }
        }

        #endregion Methods
    }
}