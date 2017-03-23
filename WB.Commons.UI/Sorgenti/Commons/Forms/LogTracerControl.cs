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
    /// Class LogTracerControl
    /// </summary>
    public partial class LogTracerControl : UserControl
    {
        #region Fields

        /// <summary>
        /// The logs
        /// </summary>
        private readonly List<Log> logs = new List<Log>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTracerControl"/> class.
        /// </summary>
        public LogTracerControl()
        {
            InitializeComponent();

            Load += LogTracerControl_Load;
        }

        #endregion Constructors

        #region Properties

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
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddLog(level, msg, parms)));
                return;
            }

            var log = new Log(DateTime.Now, level, msg, parms);
            logs.Add(log);
            lvAdd(new[] {log});
        }

        public void ClearLogs(IEnumerable<LogLevels> level = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ClearLogs(level)));
                return;
            }

            if (level == null)
                level = new []
                            {
                                LogLevels.Debug,
                                LogLevels.Disabled,
                                LogLevels.Error,
                                LogLevels.Info,
                                LogLevels.Trace,
                                LogLevels.Warning
                            };

            logs.ToList().ForEach(log=>
                                      {
                                          if (level.Contains(log.LogLevel))
                                              logs.Remove(log);
                                      });
        }

        /// <summary>
        /// Handles the Load event of the LogTracerControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LogTracerControl_Load(object sender, EventArgs e)
        {
            var logvalues = (from LogLevels entry in Enum.GetValues(typeof (LogLevels)) select entry as object);
            if (logvalues != null)
                lbLogLevels.Items.AddRange(logvalues.ToArray());

            btnAll.Click += (s1, e1)=>
                                {
                                    tbTrace.SuspendLayout();
                                    for (int i = 0; i < lbLogLevels.Items.Count; i++)
                                    {
                                        lbLogLevels.SetSelected(i, true);
                                    }
                                    tbTrace.ResumeLayout(true);
                                };

            lbLogLevels.SelectedIndexChanged += (s1, e1)=>UpdateView();
            lbLogLevels.SelectedItem = LogLevels.Info;
        }

        /// <summary>
        /// Lvs the add.
        /// </summary>
        /// <param name="_logs">The _logs.</param>
        private void lvAdd(IEnumerable<Log> _logs)
        {
            var act = new Action(()=>
                                     {
                                         var selectedLogLevels =
                                             (from object item in lbLogLevels.SelectedItems select (LogLevels) item);

                                         var logStrings = _logs.Where(
                                             log=> { return selectedLogLevels.Contains(log.LogLevel); })
                                                               .Select(
                                                                   log=>
                                                                   string.Format("{0} - {1}: {2}", log.TimeStamp,
                                                                                 log.LogLevel, log.Text));

                                         logStrings.ToList().ForEach(txt=>tbTrace.AppendText(txt + "\r\n"));
                                         ResumeLayout(true);
                                     });
            if (InvokeRequired)
            {
                Invoke(act);
            }
            else
            {
                act();
            }

            //var lvis = logs.Select(log =>
            //{
            //    if (!cboxShowAll.Checked)
            //    {
            //        switch (log.LogLevel)
            //        {
            //            case LogLevels.Debug:
            //                if (!cbDebug.Checked)
            //                    return string.Empty;
            //                break;
            //            case LogLevels.Disabled:
            //                return string.Empty;
            //            case LogLevels.Error:
            //                if (!cbError.Checked)
            //                    return string.Empty;
            //                break;
            //            case LogLevels.Info:
            //                if (!cbInfo.Checked)
            //                    return string.Empty;
            //                break;
            //            case LogLevels.Trace:
            //                if (!cbTrace.Checked)
            //                    return string.Empty;
            //                break;
            //            case LogLevels.Warning:
            //                if (!cbWarning.Checked)
            //                    return string.Empty;
            //                break;
            //            default:
            //                break;
            //        }
            //    }

            //    return string.Format("{0} - {1}: {2}", log.TimeStamp, log.LogLevel, log.Text);
            //}).Where(txt => !string.IsNullOrEmpty(txt));

            //tbTrace.Lines = lvis.ToArray();
        }

        /// <summary>
        /// Updates the view.
        /// </summary>
        private void UpdateView()
        {
            SuspendLayout();
            tbTrace.Clear();
            if (logs != null)
                lvAdd(logs);
            ResumeLayout(true);
        }

        #endregion Methods
    }
}