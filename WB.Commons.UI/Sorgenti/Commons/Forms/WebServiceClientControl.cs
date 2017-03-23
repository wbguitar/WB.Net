// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:18
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Forms
{
    using System;
    using System.Windows.Forms;

    using  WB.Commons.UI.Helpers;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// Class WebServiceClientControl
    /// </summary>
    public partial class WebServiceClientControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceClientControl"/> class.
        /// </summary>
        public WebServiceClientControl()
        {
            InitializeComponent();

            OnConnect += (msg)=> { this.DoInvoke(()=>tbWSAddress.Enabled = false); };

            OnDisconnect += ()=> { this.DoInvoke(()=>tbWSAddress.Enabled = true); };

            cbConnect.CheckedChanged += (s, e)=>
                                            {
                                                if (cbConnect.Checked)
                                                    OnConnect(WsUrl);
                                                else
                                                    OnDisconnect();
                                            };

            //Load += (s,e) => tbWSAddress.DataBindings.Add("Text", this, "WsUrl");
            tbWSAddress.DataBindings.Add("Text", this, "WsUrl");
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
            this.DoInvoke(()=>
                              {
                                  cbConnect.Checked = true;
                                  tbWSAddress.Enabled = false;
                              });

            //if (InvokeRequired)
            //{

            //    Invoke(new Action(() => cbConnect.Checked = true));
            //}
            //else
            //{
            //    cbConnect.Checked = true;
            //}
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            this.DoInvoke(()=>
                              {
                                  cbConnect.Checked = false;
                                  tbWSAddress.Enabled = true;
                              });

            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() => cbConnect.Checked = false));
            //}
            //else
            //{
            //    cbConnect.Checked = false;
            //}
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