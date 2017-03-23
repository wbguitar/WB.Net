// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:18
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.UI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;

    using  WB.Commons.Forms;
    using  WB.Commons.Helpers;

    /// <summary>
    /// Class Misc
    /// </summary>
    public static class Misc
    {
        #region Methods

        /// <summary>
        /// Does the invoke.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void DoInvoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }

        /// <summary>
        /// Checks equality for a <see cref="ComboBoxItem" /> value and an object
        /// </summary>
        /// <typeparam name="T">Type of the value in the combo box item</typeparam>
        /// <param name="cbi">The cbi.</param>
        /// <param name="obj">objetct to check equality to</param>
        /// <returns>true if ComboBoxItem's value is equal to obj</returns>
        public static bool Equals<T>(this ComboBoxItem<T> cbi, object obj)
        {
            var type = obj.GetType();
            if (!type.Equals(typeof(T)))
                return false;

            return cbi.Value.CheckEquals(obj);
        }

        /// <summary>
        /// Stores an object of type <typeparamref name="T" /> serializing it to a file selected through a dialog box
        /// </summary>
        /// <typeparam name="T">The type of object to store</typeparam>
        /// <param name="t">Object to store</param>
        /// <param name="dialogFilter">Filter string for the dialog box</param>
        public static void ExportObj<T>(this T t, string dialogFilter)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = dialogFilter;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    var ser = new XmlSerializer(t.GetType());
                    using (var writer = XmlWriter.Create(sfd.FileName, settings))
                    {
                        ser.Serialize(writer, t);
                    }
                }
                catch (Exception exc)
                {
                    var msg = string.Format("Error exporting objects {0}: {1}", typeof(T).FullName, exc);
                    System.Diagnostics.Debug.WriteLine(msg);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Stores a collection of <typeparamref name="T" /> objects serializing them in a file selected through a dialog box
        /// </summary>
        /// <typeparam name="T">The type of objects to be retrieved</typeparam>
        /// <param name="tasks">Collection of objects to save</param>
        /// <param name="dialogFilter">Filter string for the dialog box</param>
        /// <remarks>The file to be opened should be an xml serialization of a List of <typeparamref name="T" /> objects
        /// <see cref="ExportObjs" /></remarks>
        public static void ExportObjs<T>(this IEnumerable<T> tasks, string dialogFilter)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = dialogFilter;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    var ser = new XmlSerializer(typeof(List<T>));
                    using (var writer = XmlWriter.Create(sfd.FileName, settings))
                    {
                        ser.Serialize(writer, tasks.ToList());
                    }
                }
                catch (Exception exc)
                {
                    var msg = string.Format("Error exporting objects {0}: {1}", typeof(T).FullName, exc);
                    System.Diagnostics.Debug.WriteLine(msg);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Retrieves an object of type <typeparamref name="T" /> deserializing it from a file selected through a dialog box
        /// </summary>
        /// <typeparam name="T">The type of object to be retrieved</typeparam>
        /// <param name="dialogFilter">Filter string for the dialog box</param>
        /// <returns>The deserialized object</returns>
        public static T ImportObj<T>(string dialogFilter)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = dialogFilter;
            T t = default(T);
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    using (var sr = XmlReader.Create(ofd.FileName, settings))
                    {
                        var ser = new XmlSerializer(t.GetType());
                        t = (T)ser.Deserialize(sr);
                    }
                }
                catch (Exception exc)
                {
                    var msg = string.Format("Error importing objects {0}: {1}", typeof(T).FullName, exc);
                    System.Diagnostics.Debug.WriteLine(msg);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return t;
        }

        /// <summary>
        /// Retrieves an object of type <typeparamref name="T" /> deserializing it from a file selected through a dialog box
        /// </summary>
        /// <typeparam name="T">The type of object to be retrieved</typeparam>
        /// <param name="t">The t.</param>
        /// <param name="dialogFilter">Filter string for the dialog box</param>
        /// <returns>The deserialized object</returns>
        public static T ImportObj<T>(this T t, string dialogFilter)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = dialogFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    using (var sr = XmlReader.Create(ofd.FileName, settings))
                    {
                        var ser = new XmlSerializer(t.GetType());
                        t = (T)ser.Deserialize(sr);
                    }
                }
                catch (Exception exc)
                {
                    var msg = string.Format("Error importing objects {0}: {1}", typeof(T).FullName, exc);
                    System.Diagnostics.Debug.WriteLine(msg);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return t;
        }

        /// <summary>
        /// Retrieves an array of <typeparamref name="T" /> objects deserializing them from a file selected through a dialog box
        /// </summary>
        /// <typeparam name="T">The type of objects to be retrieved</typeparam>
        /// <param name="objs">The objs.</param>
        /// <param name="dialogFilter">Filter string for the dialog box</param>
        /// <returns>The array of <typeparamref name="T" /> objects deserialized form</returns>
        /// <remarks>The file to be opened should be an xml serialization of a List of <typeparamref name="T" /> objects
        /// <see cref="ExportObjs" /></remarks>
        public static T[] ImportObjs<T>(this IEnumerable<T> objs, string dialogFilter)
        {
            return ImportObjs<T>(dialogFilter).ToArray();
        }

        /// <summary>
        /// Retrieves a List of <typeparamref name="T" /> objects deserializing them from a file selected through a dialog box
        /// </summary>
        /// <typeparam name="T">The type of objects to be retrieved</typeparam>
        /// <param name="dialogFilter">Filter string for the dialog box</param>
        /// <returns>The collection of <typeparamref name="T" /> objects deserialized form</returns>
        /// <remarks>The file to be opened should be an xml serialization of a List of <typeparamref name="T" /> objects
        /// <see cref="ExportObjs" /></remarks>
        public static IEnumerable<T> ImportObjs<T>(string dialogFilter)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = dialogFilter;
            List<T> tasks = null;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    using (var sr = XmlReader.Create(ofd.FileName, settings))
                    {
                        var ser = new XmlSerializer(typeof(List<T>));
                        tasks = (List<T>)ser.Deserialize(sr);
                    }
                }
                catch (Exception exc)
                {
                    var msg = string.Format("Error importing objects {0}: {1}", typeof(T).FullName, exc);
                    System.Diagnostics.Debug.WriteLine(msg);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    yield return task;
                }
            }
        }

        /// <summary>
        /// Creates a ComboBoxItem of type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>ComboBoxItem{``0}.</returns>
        public static ComboBoxItem<T> ToComboBoxItem<T>(this T obj)
        {
            var item = new ComboBoxItem<T>(obj);
            return item;
        }

        /// <summary>
        /// To the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lv">The lv.</param>
        /// <returns>IEnumerable{``0}.</returns>
        public static IEnumerable<T> ToList<T>(this ListView lv)
        {
            var items = (from ListViewItem lvi in lv.Items select lvi);
            return items.Cast<T>().ToList();
        }

        #endregion Methods


        #region Controls extensions
        public static void WBInvoke(this Control ctrl, Action act)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(act);
                return;
            }

            act();
        }


        public static void WBInvoke<T1>(this Control ctrl, Action<T1> act, T1 p1)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new Action(() => act(p1)));
                return;
            }

            act(p1);
        }

        public static void WBInvoke<T1, T2>(this Control ctrl, Action<T1, T2> act, T1 p1, T2 p2)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new Action(() => act(p1, p2)));
                return;
            }

            act(p1, p2);
        }


        public static TResult WBInvoke<TResult>(this Control ctrl, Func<TResult> func)
        {
            if (ctrl.InvokeRequired)
            {
                var res = ctrl.Invoke(func);
                return (TResult)res;
            }

            return func();
        }
        #endregion
    }
}