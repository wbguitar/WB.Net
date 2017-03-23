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
    using System.Windows.Forms;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// Class WSClientForm
    /// </summary>
    public partial class WSClientForm : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WSClientForm"/> class.
        /// </summary>
        public WSClientForm()
        {
            InitializeComponent();

            tbWSAddress.DataBindings.Add("Text", this, "WsUrl");

            cbConnect.CheckedChanged += (s, e)=>
                                            {
                                                if (cbConnect.Checked)
                                                    OnConnect(WsUrl);
                                                else
                                                    OnDisconnect();
                                            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the log control.
        /// </summary>
        /// <value>The log control.</value>
        public LogTracerControl LogControl
        {
            get { return logTracerControl1; }
        }

        /// <summary>
        /// Gets or sets the ws URL.
        /// </summary>
        /// <value>The ws URL.</value>
        public string WsUrl
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            cbConnect.Checked = true;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            cbConnect.Checked = false;
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="parms">The parms.</param>
        public void Log(LogLevels level, string msg, params object[] parms)
        {
            try
            {
                if (Created && IsHandleCreated)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(()=>LogControl.AddLog(level, msg, parms)));
                    }
                    else
                        LogControl.AddLog(level, msg, parms);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
                throw;
            }
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Occurs when [on connect].
        /// </summary>
        public event Action<string> OnConnect = (s) => { };
        /// <summary>
        /// Occurs when [on disconnect].
        /// </summary>
        public event Action OnDisconnect = () => { };
        

        #endregion Other
    }
}