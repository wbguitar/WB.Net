// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:19
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// Class LogControl
    /// </summary>
    public partial class LogControl : UserControl
    {
        #region Fields

        /// <summary>
        /// The logs
        /// </summary>
        private readonly List<Log> logs = new List<Log>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogControl"/> class.
        /// </summary>
        public LogControl()
        {
            InitializeComponent();

            cboxShowAll.CheckedChanged += (s, e)=>
                                              {
                                                  groupBox1.Enabled = !cboxShowAll.Checked;
                                                  UpdateView();
                                              };
            cbDebug.CheckedChanged   += (s, e)=>UpdateView();
            cbError.CheckedChanged   += (s, e)=>UpdateView();
            cbInfo.CheckedChanged    += (s, e)=>UpdateView();
            cbTrace.CheckedChanged   += (s, e)=>UpdateView();
            cbWarning.CheckedChanged += (s, e)=>UpdateView();

            cbDebug.DataBindings.Add(new Binding("Checked", this, "LogLevelDebug"));
            cbInfo.DataBindings.Add(new Binding("Checked", this, "LogLevelDebug"));
            cbTrace.DataBindings.Add(new Binding("Checked", this, "LogLevelTrace"));
            cbError.DataBindings.Add(new Binding("Checked", this, "LogLevelError"));
            cbWarning.DataBindings.Add(new Binding("Checked", this, "LogLevelWarning"));
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [log level debug].
        /// </summary>
        /// <value><c>true</c> if [log level debug]; otherwise, <c>false</c>.</value>
        public bool LogLevelDebug
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [log level error].
        /// </summary>
        /// <value><c>true</c> if [log level error]; otherwise, <c>false</c>.</value>
        public bool LogLevelError
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [log level info].
        /// </summary>
        /// <value><c>true</c> if [log level info]; otherwise, <c>false</c>.</value>
        public bool LogLevelInfo
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [log level trace].
        /// </summary>
        /// <value><c>true</c> if [log level trace]; otherwise, <c>false</c>.</value>
        public bool LogLevelTrace
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [log level warning].
        /// </summary>
        /// <value><c>true</c> if [log level warning]; otherwise, <c>false</c>.</value>
        public bool LogLevelWarning
        {
            get; set;
        }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <value>The logs.</value>
        private List<Log> Logs
        {
            get { return logs; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="parms">The parms.</param>
        public void AddLog(LogLevels level, string msg, params object[] parms)
        {
            var log = new Log(DateTime.Now, level, msg, parms);
            logs.Add(log);
            lvAdd(new[] {log});
        }

        /// <summary>
        /// Lvs the add.
        /// </summary>
        /// <param name="logs">The logs.</param>
        private void lvAdd(IEnumerable<Log> logs)
        {
            var lvis = logs.Select(log =>
            {
                if (!cboxShowAll.Checked)
                {
                    switch (log.LogLevel)
                    {
                        case LogLevels.Debug:
                            if (!cbDebug.Checked)
                                return null;
                            break;
                        case LogLevels.Disabled:
                            return null;
                        case LogLevels.Error:
                            if (!cbError.Checked)
                                return null;
                            break;
                        case LogLevels.Info:
                            if (!cbInfo.Checked)
                                return null;
                            break;
                        case LogLevels.Trace:
                            if (!cbTrace.Checked)
                                return null;
                            break;
                        case LogLevels.Warning:
                            if (!cbWarning.Checked)
                                return null;
                            break;
                        default:
                            break;
                    }
                }

                var lvi = new ListViewItem();
                lvi.Text = lvi.Name = log.TimeStamp.ToString("yyyy/MM/dd HH:mm:ss");
                lvi.SubItems.Add(log.LogLevel.ToString());
                lvi.SubItems.Add(log.Text);
                //lvLogs.Items.Add(lvi);

                return lvi;
            }).Where(lvi => lvi != null);

            if (lvLogs.InvokeRequired)
            {
                lvLogs.Invoke(
                    new Action(()
                               =>
                    {
                        lvLogs.BeginUpdate();
                        lvLogs.Items.AddRange(lvis.ToArray());
                        lvLogs.EndUpdate();
                    }));
            }
            else
            {
                lvLogs.BeginUpdate();
                lvLogs.Items.AddRange(lvis.ToArray());
                lvLogs.EndUpdate();
            }
        }

        /// <summary>
        /// Updates the view.
        /// </summary>
        private void UpdateView()
        {
            lvAdd(logs);
        }

        #endregion Methods
    }
}