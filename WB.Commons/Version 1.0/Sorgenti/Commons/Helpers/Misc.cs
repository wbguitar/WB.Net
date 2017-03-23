// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:17
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Class Misc
    /// </summary>
    public static class Misc
    {
        #region Methods

        /// <summary>
        /// Asses the qual name split.
        /// </summary>
        /// <param name="aqname">The aqname.</param>
        /// <returns>AssemblyInfo.</returns>
        public static AssemblyInfo AssQualNameSplit(this string aqname)
        {
            List<string> parts = aqname.Split(' ')
                                       .Select(x=>x.Trim(", ".ToCharArray()))
                                       .ToList();

            string name = parts[0];
            string assembly = parts.Count < 2 ? string.Empty : parts[1];
            string version = parts.Count < 3 ? string.Empty : parts[2];
            string culture = parts.Count < 4 ? string.Empty : parts[3];
            string token = parts.Count < 5 ? string.Empty : parts[4];

            return new AssemblyInfo
                       {
                           Name = name,
                           Assembly = assembly,
                           Version = version,
                           Culture = culture,
                           Token = token
                       };
        }

        /// <summary>
        /// Checks equality between two generic objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="objcomp">The objcomp.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool CheckEquals<T>(this T obj, object objcomp)
        {
            // verifico se sono lo stesso oggetto (stesso reference)
            if (Object.ReferenceEquals(obj, objcomp))
                return true;
            try
            {
                var tobj = (T) objcomp; // se lancia invalid cast sono diversi

                if (tobj == null)
                    return false;

                var type = obj.GetType();
                if (type == typeof (string) ||
                    type == typeof (DateTime) ||
                    type == typeof (TimeSpan) ||
                    type.IsEnum ||
                    type.IsPrimitive)
                    return obj.Equals(objcomp);

                if (type.IsValueType)
                {
                    var fields = type.GetFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        var v1 = field.GetValue(obj);
                        // per qualche ragione tra i field di un valuetype c'e` anche l'oggetto,
                        // lo skippo altrimenti vado in stack overflow
                        if (v1.Equals(obj))
                            continue;

                        var v2 = field.GetValue(objcomp);

                        if (!v1.CheckEquals(v2))
                            return false;
                    }
                    // 					foreach (var field in fields)
                    // 					{
                    // 						var v1 = field.GetValue(obj);
                    // 						var v2 = field.GetValue(objcomp);
                    // 						if (!v1.CheckEquals(v2))
                    // 							return false;
                    // 					}

                    return true;
                }
                var props = obj.GetType().GetProperties();

                //if (props.Length == 0) // tipo primitivo
                //{
                //    return obj.Equals(tobj);
                //}

                foreach (var prop in props)
                {
                    var parms = prop.GetIndexParameters();
                    if (parms.Length != 0) // e` una collection
                    {
                        foreach (var pi in parms)
                        {
                            var vit1 = prop.GetValue(obj, new object[] {pi});
                            var vit2 = prop.GetValue(tobj, new object[] {pi});

                            if (!vit1.CheckEquals(vit2))
                                return false;
                        }
                    }

                    else
                    {
                        var val1 = prop.GetValue(obj, null);
                        var val2 = prop.GetValue(tobj, null);

                        if (val1 == null)
                            return val2 == null;

                        if (prop.PropertyType.Equals(typeof (string)) ||
                            prop.PropertyType.IsPrimitive)
                        {
                            if (!val1.Equals(val2))
                                return false;
                        }
                        else if (typeof (IEnumerable).IsAssignableFrom(prop.PropertyType))
                        {
                            var ie1 = (val1 as IEnumerable);
                            var ie2 = (val2 as IEnumerable);
                            foreach (var i1 in ie1)
                            {
                                bool found = false;
                                foreach (var i2 in ie2)
                                {
                                    if (i1.CheckEquals(i2))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                    return false;
                            }
                        }
                        else if (prop.PropertyType is IDictionary)
                        {
                        }
                        else
                        {
                            if (!val1.CheckEquals(val2))
                                return false;
                        }
                    }
                }
            }
            catch (System.InvalidCastException)
            {
                return false;
            }
            catch (System.Reflection.TargetException)
            {
                return false;
            }

            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                if (exc.InnerException != null && exc.InnerException is NotImplementedException)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("Not implemented exception: source={0}, message={1}", exc.Source, exc.Message));
                }
            }

            return true;
        }

        /// <summary>
        /// Create a textual representation of an object
        /// </summary>
        /// <param name="obj">Object to textualize</param>
        /// <param name="name">Name of the object</param>
        /// <param name="indent">Current indent level</param>
        /// <returns>The string representing the object</returns>
        public static string DisplayString(this object obj, string name = null, int indent = 0)
        {
            //var strindent = Enumerable.Range(0, indent).Aggregate<int, string>(" ", (s1, s2) => s1 += s2);

            var strindent = "";
            if (indent > 0)
                strindent = Enumerable.Range(0, 3).Aggregate(" ", (sin, i)=>sin += " ");
            var str = string.Format("{0}{1}: ", strindent, string.IsNullOrEmpty(name) ? "Value" : name);
            if (obj == null)
                str += "null";
            else
                try
                {
                    var type = obj.GetType();
                    if (type.Equals(typeof (string)))
                        str += string.Format("\'{0}\' ({1})", obj, type.Name);
                    else if (type.IsPrimitive || type.IsValueType)
                        str += string.Format("{0} ({1})", obj, type.Name);
                    else if (type.IsArray)
                    {
                        str += "[ ";
                        var objarr = (obj as Array);

                        foreach (var item in objarr)
                        {
                            //str += item.DisplayString(null, indent + 1) + ", ";
                            str += item + ", ";
                        }
                        str += " ]";
                    }
                    else if (type.IsCollection())
                    {
                        str += "[ ";
                        foreach (var item in (obj as IEnumerable))
                        {
                            //str += item.DisplayString(null, indent + 1) + ", ";
                            str += item + ", ";
                        }
                        str += " ]";
                    }
                    else if (type.IsDictionary())
                    {
                        str += "[ ";
                        foreach (var key in (obj as IDictionary).Keys)
                        {
                            var item = (obj as IDictionary)[key];
                            str += string.Format("{0}:{1}, ", key, obj);
                        }
                        str += " ]";
                    }
                    else
                    {
                        var props = obj.GetType().GetProperties();
                        var sprop = string.Format(@"
            {0}{{
            ", strindent);
                        foreach (var prop in props)
                        {
                            var val1 = prop.GetValue(obj, null);
                            sprop += val1.DisplayString(prop.Name, indent + 1);
                            sprop += "\r\n";

                            //var parms = prop.GetIndexParameters();

                            //if (parms.Length != 0) // e` una collection
                            //{
                            //    sprop += "[ ";
                            //    foreach (var pi in parms)
                            //    {
                            //        var vit1 = prop.GetValue(obj, new object[] { pi });
                            //        sprop += vit1.DisplayString(prop.Name, indent + 1) + ", ";
                            //    }
                            //    sprop += " ]";
                            //}
                            //else
                            //{
                            //    var val1 = prop.GetValue(obj, null);
                            //    sprop += val1.DisplayString(prop.Name, indent + 1);
                            //}
                            //sprop += "\r\n";
                        }

                        sprop += string.Format(@"{0}}}", strindent);
                        str += sprop;
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    str = "Error: " + ex;
                }

            return string.Format("{0}{1}", strindent, str);
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <param name="aqname">The aqname.</param>
        /// <returns>AssemblyName.</returns>
        public static AssemblyName GetAssemblyName(this string aqname)
        {
            try
            {
                return new AssemblyName(aqname);
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                return null;
            }
        }

        /// <summary>
        /// Determines whether the specified t is collection.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns><c>true</c> if the specified t is collection; otherwise, <c>false</c>.</returns>
        public static bool IsCollection(this Type t)
        {
            List<Type> collections = new List<Type> {typeof (IEnumerable<>), typeof (IEnumerable)};

            if (!t.Equals(typeof (string)) &&
                (t.GetInterfaces().Any(i=>collections.Any(c=>i == c)) ||
                 t.IsArray))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified t is dictionary.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns><c>true</c> if the specified t is dictionary; otherwise, <c>false</c>.</returns>
        public static bool IsDictionary(this Type t)
        {
            if (!t.Equals(typeof (string)) && t.GetInterfaces().Any(i=>i.Name.Contains("IDictionary")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Serializations the test.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph">The graph.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool SerializationTest<T>(T graph)
        {
            bool ok = false;
            try
            {
                var ostream = new System.IO.MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Serialize(ostream, graph);
                ostream.Close();

                var istream = new System.IO.MemoryStream(ostream.GetBuffer());
                T xjd1 = (T) formatter.Deserialize(istream);
                ok = xjd1.Equals(graph);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }
            return ok;
        }

        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SerializeToXml(object value)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            XmlSerializer serializer = new XmlSerializer(value.GetType());
            serializer.Serialize(writer, value);
            return writer.ToString();
        }

        /// <summary>
        /// Swaps the specified t1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T temp = t1;
            t1 = t2;
            t2 = temp;
        }

        /// <summary>
        /// XMLs the serialization test.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph">The graph.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool XmlSerializationTest<T>(T graph)
        {
            bool ok = false;
            try
            {
                var ostream = new System.IO.MemoryStream();
                var ser = new XmlSerializer(typeof (T));
                ser.Serialize(ostream, graph);
                ostream.Close();

                var istream = new System.IO.MemoryStream(ostream.GetBuffer());
                T xjd1 = (T) ser.Deserialize(istream);
                ok = xjd1.Equals(graph);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }
            return ok;
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Struct AssemblyInfo
        /// </summary>
        public struct AssemblyInfo
        {
            #region Properties

            /// <summary>
            /// Gets or sets the assembly.
            /// </summary>
            /// <value>The assembly.</value>
            public string Assembly
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the culture.
            /// </summary>
            /// <value>The culture.</value>
            public string Culture
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the token.
            /// </summary>
            /// <value>The token.</value>
            public string Token
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            public string Version
            {
                get; set;
            }

            #endregion Properties
        }

        #endregion Nested Types

        #region Other

        //#region Forms
        ///// <summary>
        /////     Stores a collection of <typeparamref name="T" /> objects serializing them in a file selected through a dialog box
        ///// </summary>
        ///// <typeparam name="T"> The type of objects to be retrieved </typeparam>
        ///// <param name="tasks"> Collection of objects to save </param>
        ///// <param name="dialogFilter"> Filter string for the dialog box </param>
        ///// <remarks>
        /////     The file to be opened should be an xml serialization of a List of <typeparamref name="T" /> objects
        /////     <see cref="ExportObjs" />
        ///// </remarks>
        //public static void ExportObjs<T>(this IEnumerable<T> tasks, string dialogFilter)
        //{
        //    var sfd = new SaveFileDialog();
        //    sfd.Filter = dialogFilter;
        //    if (sfd.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            var settings = new XmlWriterSettings();
        //            settings.Indent = true;
        //            settings.NewLineOnAttributes = true;
        //            var ser = new XmlSerializer(typeof (List<T>));
        //            using (var writer = XmlWriter.Create(sfd.FileName, settings))
        //            {
        //                ser.Serialize(writer, tasks.ToList());
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            var msg = string.Format("Error exporting objects {0}: {1}", typeof (T).FullName, exc);
        //            System.Diagnostics.Debug.WriteLine(msg);
        //            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}
        ///// <summary>
        /////     Stores an object of type <typeparamref name="T" /> serializing it to a file selected through a dialog box
        ///// </summary>
        ///// <typeparam name="T"> The type of object to store </typeparam>
        ///// <param name="t"> Object to store </param>
        ///// <param name="dialogFilter"> Filter string for the dialog box </param>
        //public static void ExportObj<T>(this T t, string dialogFilter)
        //{
        //    var sfd = new SaveFileDialog();
        //    sfd.Filter = dialogFilter;
        //    if (sfd.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            var settings = new XmlWriterSettings();
        //            settings.Indent = true;
        //            settings.NewLineOnAttributes = true;
        //            var ser = new XmlSerializer(t.GetType());
        //            using (var writer = XmlWriter.Create(sfd.FileName, settings))
        //            {
        //                ser.Serialize(writer, t);
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            var msg = string.Format("Error exporting objects {0}: {1}", typeof (T).FullName, exc);
        //            System.Diagnostics.Debug.WriteLine(msg);
        //            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}
        ///// <summary>
        /////     Retrieves an array of <typeparamref name="T" /> objects deserializing them from a file selected through a dialog box
        ///// </summary>
        ///// <typeparam name="T"> The type of objects to be retrieved </typeparam>
        ///// <param name="dialogFilter"> Filter string for the dialog box </param>
        ///// <param name="obj"> Object of the same type of the objects to import </param>
        ///// <returns>
        /////     The array of <typeparamref name="T" /> objects deserialized form
        ///// </returns>
        ///// <remarks>
        /////     The file to be opened should be an xml serialization of a List of <typeparamref name="T" /> objects
        /////     <see cref="ExportObjs" />
        ///// </remarks>
        //public static T[] ImportObjs<T>(this IEnumerable<T> objs, string dialogFilter)
        //{
        //    return ImportObjs<T>(dialogFilter).ToArray();
        //}
        ///// <summary>
        /////     Retrieves a List of <typeparamref name="T" /> objects deserializing them from a file selected through a dialog box
        ///// </summary>
        ///// <typeparam name="T"> The type of objects to be retrieved </typeparam>
        ///// <param name="dialogFilter"> Filter string for the dialog box </param>
        ///// <returns>
        /////     The collection of <typeparamref name="T" /> objects deserialized form
        ///// </returns>
        ///// <remarks>
        /////     The file to be opened should be an xml serialization of a List of <typeparamref name="T" /> objects
        /////     <see cref="ExportObjs" />
        ///// </remarks>
        //public static IEnumerable<T> ImportObjs<T>(string dialogFilter)
        //{
        //    var ofd = new OpenFileDialog();
        //    ofd.Filter = dialogFilter;
        //    List<T> tasks = null;
        //    if (ofd.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            var settings = new XmlReaderSettings();
        //            settings.IgnoreWhitespace = true;
        //            using (var sr = XmlReader.Create(ofd.FileName, settings))
        //            {
        //                var ser = new XmlSerializer(typeof (List<T>));
        //                tasks = (List<T>) ser.Deserialize(sr);
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            var msg = string.Format("Error importing objects {0}: {1}", typeof (T).FullName, exc);
        //            System.Diagnostics.Debug.WriteLine(msg);
        //            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //    if (tasks != null)
        //    {
        //        foreach (var task in tasks)
        //        {
        //            yield return task;
        //        }
        //    }
        //}
        ///// <summary>
        /////     Retrieves an object of type <typeparamref name="T" /> deserializing it from a file selected through a dialog box
        ///// </summary>
        ///// <typeparam name="T"> The type of object to be retrieved </typeparam>
        ///// <param name="dialogFilter"> Filter string for the dialog box </param>
        ///// <returns> The deserialized object </returns>
        //public static T ImportObj<T>(string dialogFilter)
        //{
        //    var ofd = new OpenFileDialog();
        //    ofd.Filter = dialogFilter;
        //    T t = default(T);
        //    if (ofd.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            var settings = new XmlReaderSettings();
        //            settings.IgnoreWhitespace = true;
        //            using (var sr = XmlReader.Create(ofd.FileName, settings))
        //            {
        //                var ser = new XmlSerializer(t.GetType());
        //                t = (T) ser.Deserialize(sr);
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            var msg = string.Format("Error importing objects {0}: {1}", typeof (T).FullName, exc);
        //            System.Diagnostics.Debug.WriteLine(msg);
        //            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //    return t;
        //}
        ///// <summary>
        /////     Retrieves an object of type <typeparamref name="T" /> deserializing it from a file selected through a dialog box
        ///// </summary>
        ///// <typeparam name="T"> The type of object to be retrieved </typeparam>
        ///// <param name="dialogFilter"> Filter string for the dialog box </param>
        ///// <returns> The deserialized object </returns>
        //public static T ImportObj<T>(this T t, string dialogFilter)
        //{
        //    var ofd = new OpenFileDialog();
        //    ofd.Filter = dialogFilter;
        //    if (ofd.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            var settings = new XmlReaderSettings();
        //            settings.IgnoreWhitespace = true;
        //            using (var sr = XmlReader.Create(ofd.FileName, settings))
        //            {
        //                var ser = new XmlSerializer(t.GetType());
        //                t = (T) ser.Deserialize(sr);
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            var msg = string.Format("Error importing objects {0}: {1}", typeof (T).FullName, exc);
        //            System.Diagnostics.Debug.WriteLine(msg);
        //            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //    return t;
        //}
        ///// <summary>
        /////     Creates a ComboBoxItem of type <typeparamref name="T" />
        ///// </summary>
        ///// <typeparam name="T"> </typeparam>
        ///// <param name="obj"> </param>
        ///// <returns> </returns>
        //public static ComboBoxItem<T> ToComboBoxItem<T>(this T obj)
        //{
        //    var item = new ComboBoxItem<T>(obj);
        //    return item;
        //}
        ///// <summary>
        /////     Checks equality for a <see cref="ComboBoxItem" /> value and an object
        ///// </summary>
        ///// <typeparam name="T"> Type of the value in the combo box item </typeparam>
        ///// <param name="cbi"> </param>
        ///// <param name="obj"> objetct to check equality to </param>
        ///// <returns> true if ComboBoxItem's value is equal to obj </returns>
        //public static bool Equals<T>(this ComboBoxItem<T> cbi, object obj)
        //{
        //    var type = obj.GetType();
        //    if (!type.Equals(typeof(T)))
        //        return false;
        //    return cbi.Value.CheckEquals(obj);
        //}
        //public static void DoInvoke(this Control control, Action action)
        //{
        //    if (control.InvokeRequired)
        //        control.Invoke(action);
        //    else
        //        action();
        //}
        //#endregion

        #endregion Other
    }
}