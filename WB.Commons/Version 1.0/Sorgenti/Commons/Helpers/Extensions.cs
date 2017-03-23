// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:17
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace WB.Commons.Helpers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Class Extensions
    /// </summary>
    public static class Extensions
    {
        #region Methods

        private static string defaultXmlNamespace = ""; // "NsDefault";
        private static Encoding defaultXmlEncoding = new UnicodeEncoding();
        //private static Encoding defaultXmlEncoding = new UTF8Encoding();

        /// <summary>
        /// Deserializes the specified buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer">The buffer.</param>
        /// <returns>``0.</returns>
        public static T Deserialize<T>(this byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            var bf = new BinaryFormatter();
            return (T)bf.Deserialize(ms);
        }

        /// <summary>
        /// Deserializes the specified buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer">The buffer.</param>
        /// <param name="obj">The obj.</param>
        public static void Deserialize<T>(this byte[] buffer, ref T obj)
        {
            var ms = new MemoryStream(buffer);
            var bf = new BinaryFormatter();
            obj = (T)bf.Deserialize(ms);
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>``0.</returns>
        public static T DeserializeFromXml<T>(this string xmlString, Encoding _encoding = null)
        {
            if (string.IsNullOrEmpty(xmlString))
                return default(T);


            T obj = default(T);
            var sr = new StringReader(xmlString);



            //XmlSerializer serializer = new XmlSerializer(typeof(T), defaultXmlNamespace);

            //var settings = new XmlReaderSettings();
            //settings.IgnoreWhitespace = true;
            //using (var xr = XmlReader.Create(sr, settings))
            //{
            //    if (serializer.CanDeserialize(xr))
            //    {
            //        obj = (T)serializer.Deserialize(xr);
            //    }
            //}

            var encoding = _encoding ?? defaultXmlEncoding;
            using (var memStream = new MemoryStream(encoding.GetBytes(xmlString)))
            {
                memStream.Position = 0;
                var ser = new System.Xml.Serialization.XmlSerializer(typeof(T), SerializerInfo.XmlIncludedTypes);
                obj = (T)ser.Deserialize(memStream);
            }

            return obj;
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString">The XML string.</param>
        /// <param name="obj">The obj.</param>
        public static void DeserializeFromXml<T>(this string xmlString, ref T obj, Encoding _encoding = null)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                obj = default(T);
                return;
            }

            //var sr = new StringReader(xmlString);
            var sr = new StringReader(xmlString);
            XmlSerializer serializer = null;
            Type objType = obj == null
                             ? typeof(T)
                             : obj.GetType();



            //serializer = new XmlSerializer(objType, defaultXmlNamespace);
            ////serializer = new XmlSerializer(objType);

            //var settings = new XmlReaderSettings {IgnoreWhitespace = true};
            //using (var xr = XmlReader.Create(sr, settings))
            //{
            //    if (serializer.CanDeserialize(xr))
            //    {
            //        obj = (T)serializer.Deserialize(xr);
            //    }
            //}

            var encoding = _encoding ?? defaultXmlEncoding;
            using (MemoryStream memStream = new MemoryStream(encoding.GetBytes(xmlString)))
            {
                memStream.Position = 0;
                var ser = new System.Xml.Serialization.XmlSerializer(objType, SerializerInfo.XmlIncludedTypes);
                obj = (T)ser.Deserialize(memStream);
            }
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>``0.</returns>
        public static T DeserializeFromXml<T>(this T obj, string xmlString, Encoding _encoding = null)
        {
            if (string.IsNullOrEmpty(xmlString))
                return default(T);



            Type objType = null;
            objType = obj != null ? obj.GetType() : typeof(T);



            //var serializer = new XmlSerializer(objType);
            ////var serializer = new XmlSerializer(objType, defaultXmlNamespace);

            //var settings = new XmlReaderSettings
            //                   {
            //                       IgnoreWhitespace = true,

            //                   };

            //var sr = new StringReader(xmlString);
            ////using (var xr = XmlTextReader.Create(sr, settings))
            //using (var xr = XmlReader.Create(sr))
            //{
            //    if (serializer.CanDeserialize(xr))
            //    {
            //        obj = (T)serializer.Deserialize(xr);
            //        return obj;
            //    }
            //}

            var encoding = _encoding ?? defaultXmlEncoding;
            using (MemoryStream memStream = new MemoryStream(encoding.GetBytes(xmlString)))
            {
                memStream.Position = 0;
                var ser = new System.Xml.Serialization.XmlSerializer(objType, SerializerInfo.XmlIncludedTypes);
                var t = (T)ser.Deserialize(memStream);
                return t;
            }
        }


        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <param name="serializer">The XML serializer</param>
        /// <returns>``0.</returns>
        public static T DeserializeFromXml<T>(this T obj, string xmlString, XmlSerializer serializer, Encoding _encoding = null)
        {
            if (string.IsNullOrEmpty(xmlString))
                return default(T);

            var encoding = _encoding ?? defaultXmlEncoding;
            using (var memStream = new MemoryStream(encoding.GetBytes(xmlString)))
            {
                memStream.Position = 0;
                //var ser = new System.Xml.Serialization.XmlSerializer(objType);
                var t = (T)serializer.Deserialize(memStream);
                return t;
            }
        }

        /// <summary>
        /// Finds the serializer for the specified type name string
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>XmlSerializer.</returns>
        public static XmlSerializer FindSerializer(this string xmlString,
            IEnumerable<Assembly> assemblies = null, bool onlyNonSysAssemblies = false, bool isFullName = true)
        {
            var doc = XDocument.Parse(xmlString);
            if (doc == null)
                return null;

            var type = TypesLoader.GetType(doc.Root.Name.ToString(), assemblies, onlyNonSysAssemblies, isFullName);
            if (type == null)
                return null;

            return new XmlSerializer(type, SerializerInfo.XmlIncludedTypes);
        }


        /// <summary>
        /// Params2s the XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>System.String.</returns>
        public static string Params2Xml<T>(this T obj)
        {
            var parms = "";

            if (obj == null)
                return parms;

            var type = obj.GetType();
            var props = type.GetProperties();

            if (props.Any())
            {
                var set = new XmlWriterSettings();
                set.NewLineHandling = NewLineHandling.Entitize;
                set.Indent = true;
                
                var ser = new XmlSerializer(type, SerializerInfo.XmlIncludedTypes);
                using (var sw = new StringWriter())
                {
                    ser.Serialize(XmlWriter.Create(sw, set), obj);
                    parms = sw.ToString();
                }
            }

            return parms.Trim();
        }

        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] Serialize<T>(this object obj)
        {
            var ms = new MemoryStream();
            //var bw = new BinaryWriter(ms);
            var bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms.GetBuffer();
        }
        
        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>System.String.</returns>
        public static string SerializeToXml<T>(this T value, XmlWriterSettings settings = null)
        {
            if (settings == null)
            {
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                settings.CloseOutput = true;
                settings.OmitXmlDeclaration = false;
                settings.Encoding = defaultXmlEncoding;
            }

            using (var ms = new MemoryStream())
            {
                //StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
                StringBuilder writer = new StringBuilder();

                using (var xw = XmlTextWriter.Create(ms, settings))
                {
                    //var serializer = new XmlSerializer(value.GetType(), defaultXmlNamespace);
                    //serializer.Serialize(xw, value);

                    var serializer = new XmlSerializer(value.GetType(), SerializerInfo.XmlIncludedTypes);
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", defaultXmlNamespace);
                    serializer.Serialize(xw, value, ns);

                    writer.AppendFormat("{0}", settings.Encoding.GetString(ms.ToArray()));
                    var str = writer.ToString();
                    return str;
                }
            }

            ////StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            //StringBuilder writer = new StringBuilder();

            //var xw = XmlTextWriter.Create(writer, settings);
            //XmlSerializer serializer = new XmlSerializer(value.GetType(), "");

            //var ns = new XmlSerializerNamespaces();
            //ns.Add("", "");

            //serializer.Serialize(xw, value, ns);
            ////serializer.Serialize(xw, value);

            //var str = writer.ToString();
            //return str;
        }

        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="sw">The sw.</param>
        public static void SerializeToXml<T>(this T value, StreamWriter sw)
        {
            var settings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   NewLineOnAttributes = true,
                                   CloseOutput = true,
                                   Encoding = defaultXmlEncoding,
                                   //settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                                   OmitXmlDeclaration = false,
                               };
            

            var xw = XmlWriter.Create(sw, settings);
            //var serializer = new XmlSerializer(typeof(T), defaultXmlNamespace);
            //serializer.Serialize(xw, value);

            var serializer = new XmlSerializer(value.GetType(), SerializerInfo.XmlIncludedTypes);
            var ns = new XmlSerializerNamespaces();
            ns.Add("", defaultXmlNamespace);
            serializer.Serialize(xw, value, ns);

            xw.Flush();
        }

        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.String.</returns>
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start; // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }

        /// <summary>
        /// Slices the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>``0[][].</returns>
        /// <exception cref="System.IndexOutOfRangeException">start
        /// or
        /// start + count</exception>
        public static T[] Slice<T>(this T[] source, int start, int count)
        {
            //int count = end - start + 1;
            //T[] target = null;
            //if (count > 0)
            //{
            //    target = new T[count];

            //    for (int i = start; i <= end; i++)
            //        target[i - start] = source[i];
            //}
            if (start < 0 || start > source.Length)
                throw new IndexOutOfRangeException("start");
            if (start + count < 0 || start + count > source.Length)
                throw new IndexOutOfRangeException("start + count");

            var target = new T[count];
            Array.Copy(source, 0, target, 0, count);
            return target;
        }

        /// <summary>
        /// XML2s the params.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="xmlobj">The xmlobj.</param>
        /// <returns>``0.</returns>
        public static T Xml2Params<T>(this T obj, string xmlobj)
        {
            if (string.IsNullOrEmpty(xmlobj))
            {
                obj = default(T);
                return obj;
            }
            var ser = new XmlSerializer(typeof(T), SerializerInfo.XmlIncludedTypes);
            using (var tr = new StringReader(xmlobj))
            {
                var set = new XmlReaderSettings();
                set.IgnoreWhitespace = true;
                obj = (T)ser.Deserialize(XmlReader.Create(tr, set));
            }

            return obj;
        }

        public static void SaveXml<T>(this T obj, string path)
        {
            //using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (var fs = File.Open(path, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                obj.SerializeToXml(sw);
            }
        }

        public static T LoadXml<T>(this T obj, string path, Encoding _encoding = null)
        {
            T val = default(T);
            //using (var sw = File.OpenRead(path))
            //using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(path))
            {
                val = obj.DeserializeFromXml(sr.ReadToEnd(), _encoding);
            }

            return val;
        }



        #endregion Methods
    }
}