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
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Class SerializableDictionary
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TValue">The type of the T value.</typeparam>
    [XmlRoot("dictionary")]
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        public SerializableDictionary(SerializableDictionary<TKey, TValue> dict)
            : base(dict)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        public SerializableDictionary(IDictionary<TKey, TValue> dict)
            : base(dict)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            try
            {
                XmlSerializer keySerializer = new XmlSerializer(typeof (TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof (TValue));

                bool wasEmpty = reader.IsEmptyElement;
                reader.Read();

                if (wasEmpty)
                    return;

                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("item");

                    reader.ReadStartElement("key");
                    TKey key = (TKey) keySerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    var typename = reader.GetAttribute("type");
                    //var type = Type.GetType(typename);
                    var type = WB.Commons.Helpers.TypesLoader.GetType(typename);

                    reader.ReadStartElement("value");

                    valueSerializer = new XmlSerializer(type);
                    TValue value = (TValue) valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    Add(key, value);

                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
                reader.ReadEndElement();
            }
            catch (Exception exc)
            {
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            try
            {
                XmlSerializer keySerializer = new XmlSerializer(typeof (TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof (TValue));

                foreach (TKey key in Keys)
                {
                    writer.WriteStartElement("item");

                    writer.WriteStartElement("key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();

                    writer.WriteStartElement("value");

                    TValue value = this[key];

                    // creo un serializer per ogni oggetto
                    // nel caso in cui ci fossero gerarchie di tipi
                    // e salvo anche il tipo
                    var t = value.GetType();
                    //writer.WriteAttributeString("type", t.AssemblyQualifiedName);
                    writer.WriteAttributeString("type", t.FullName);
                    valueSerializer = new XmlSerializer(t);

                    valueSerializer.Serialize(writer, value);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
            }
        }

        #endregion Methods
    }
}