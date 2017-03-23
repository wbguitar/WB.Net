namespace WB.Commons.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// ListView che implementa le funzionalità di un message log
    /// </summary>
    public class ListViewLogger : ListView, IMessageLog
    {
        #region Fields

        public Dictionary<LogLevels, bool> ActiveLevels =
            new Dictionary<LogLevels, bool>()
                {
                    {LogLevels.Debug, true},
                    {LogLevels.Error, true},
                    {LogLevels.Info, true},
                    {LogLevels.Trace, true},
                    {LogLevels.Warning, true},
                    {LogLevels.Disabled, false},
                };

        private Queue<LogEntry> logs = new Queue<LogEntry>();
        private DateTime start = DateTime.Now;
        bool stop = false;
        private Thread updater;

        #endregion Fields

        #region Constructors

        public ListViewLogger()
        {
            this.View = View.Details;
            this.Columns.Add(new ColumnHeader()
            {
                Text = "Timestamp",
                Width = 100,
            });
            this.Columns.Add(new ColumnHeader()
            {
                Text = "Log level",
                Width = 80,
            });

            this.Columns.Add(new ColumnHeader()
            {
                Text = "Caller",
                Width = 250,
            });

            this.Columns.Add(new ColumnHeader()
            {
                Text = "Log message",
                Width = 250,
            });

            this.GridLines = true;
            this.FullRowSelect = true;
            this.ShowItemToolTips = true;
            MaxLogs = 500;
            //this.HandleCreated += (s, e)=>InitThread();

            //this.Disposed += (s, e)=>
            //                     {
            //                         stop = true;
            //                         updater.Join();
            //                     };
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        private delegate void dLog(LogLevels level, object caller, string message);

        #endregion Delegates

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public LogLevels LogLevel
        {
            get
            {
                return LogLevels.Debug;
            }
            set { }
        }

        public uint MaxLogs { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool CanLog(LogLevels level)
        {
            bool result;
            try
            {
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevels level, string message)
        {
            try
            {
                Log(level, null, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public void Log(LogLevels level, object caller, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(level, caller, message)));
                return;
            }

            lock (this)
            {

                try
                {
                    if (logs.Count > MaxLogs)
                        logs.Dequeue();

                    var entry = new LogEntry() { Level = level, Caller = caller, Message = message };
                    logs.Enqueue(entry);

                    // aggiorna solo ogni due secondi
                    if ((DateTime.Now - start) > TimeSpan.FromSeconds(2))
                    {
                        UpdateView();
                        start = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // not used
        void InitThread()
        {
            updater = new Thread((o) =>
            {
                while (!stop)
                {
                    this.Invoke(
                        new Action(() =>
                        {
                            lock (this)
                            {
                                this.BeginUpdate();
                                this.Items.Clear();
                                logs.ToList().ForEach(l =>
                                {
                                    if (this.InvokeRequired)
                                    {
                                        this.BeginInvoke(new dLog(Log), new object[3] { l.Level, l.Caller, l.Message });
                                        return;
                                    }

                                    ListViewItem logLine = new ListViewItem();
                                    logLine.Text = DateTime.Now.ToString();
                                    logLine.SubItems.Add(l.Level.ToString());
                                    logLine.SubItems.Add(l.Message);

                                    //this.Items.Insert(0, logLine);
                                    this.Items.Add(logLine);
                                });
                                this.EndUpdate();
                            }
                        }));

                    Thread.Sleep(1000);
                }
            });

            updater.Start(logs);
        }

        private void UpdateView()
        {
            if (this.InvokeRequired)
            {
                //this.BeginInvoke(new dLog(Log), new object[3] { level, caller, message });
                this.BeginInvoke(new Action(UpdateView));
                return;
            }

            lock (this)
            {
                this.BeginUpdate();
                try
                {
                    foreach (ListViewItem item in Items)
                    {
                        item.SubItems.Clear();
                    }

                    this.Items.Clear();
                    var toAdd = new List<ListViewItem>();
                    foreach (var entry in logs.ToList())
                    {
                        if (!ActiveLevels[entry.Level])
                            continue;

                        ListViewItem logLine = new ListViewItem();
                        logLine.Text = entry.TimeStamp.ToString();
                        logLine.SubItems.Add(entry.Level.ToString());
                        logLine.SubItems.Add(entry.Caller != null ? entry.Caller.ToString() : "");
                        logLine.SubItems.Add(entry.Message);
                        logLine.ToolTipText = entry.Message;

                        //this.Items.Insert(0, logLine);
                        toAdd.Insert(0, logLine);
                    }
                    Items.AddRange(toAdd.ToArray());
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.Print("ListViewLogger exception: {0}", exc);
                }

                this.EndUpdate();
            }
        }

        #endregion Methods

        #region Nested Types

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="listView"></param>
        //public ListViewLogger(ListView _listView)
        //{
        //    this.listView = _listView;
        //}
        class LogEntry
        {
            public LogEntry()
            {
                TimeStamp = DateTime.Now;
            }

            #region Properties

            public object Caller
            {
                get;
                set;
            }

            public LogLevels Level
            {
                get;
                set;
            }

            public string Message
            {
                get;
                set;
            }

            public DateTime TimeStamp { get; private set; }

            #endregion Properties
        }

        #endregion Nested Types

        #region Other

        /// <summary>
        /// 
        /// </summary>
        //private ListView listView;

        #endregion Other
    }
}