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
    using System.Net;
    using System.Windows.Forms;

    using WB.IIIParty.Commons.Logger;

    /// <summary>
    /// Class TcpClientForm
    /// </summary>
    public partial class TcpClientForm : Form
    {
        #region Fields

        /// <summary>
        /// The log tracer control
        /// </summary>
        private readonly LogTracerControl logTracerControl = new WB.Commons.Forms.LogTracerControl();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientForm"/> class.
        /// </summary>
        public TcpClientForm()
        {
            InitializeComponent();

            tbPort.ValueChanged += (s, e)=>Port = (int) tbPort.Value;
            tbIp.TextChanged += (s, e)=>
                                    {
                                        IPAddress ip = null;
                                        if (!IPAddress.TryParse(tbIp.Text, out ip))
                                        {
                                            MessageBox.Show("Wrong IP format", "Error", MessageBoxButtons.OK,
                                                            MessageBoxIcon.Error);
                                            tbIp.Text = IP;
                                            return;
                                        }

                                        IP = tbIp.Text;
                                    };

            cbConnect.CheckedChanged += (s, e)=>
                                            {
                                                tbIp.Enabled = tbPort.Enabled = !cbConnect.Checked;
                                                if (cbConnect.Checked)
                                                {
                                                    OnConnect();
                                                }
                                                else
                                                {
                                                    OnDisconnect();
                                                }
                                            };

            Load += (s, e)=>
                        {
                            logTracerControl.Dock = DockStyle.Fill;
                            panel1.Controls.Add(logTracerControl);
                        };

            tbPort.Value = 666;
            tbIp.Text = "127.0.0.1";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the IP.
        /// </summary>
        /// <value>The IP.</value>
        public string IP
        {
            get; protected set;
        }

        /// <summary>
        /// Gets the log control.
        /// </summary>
        /// <value>The log control.</value>
        public LogTracerControl LogControl
        {
            get { return logTracerControl; }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get; protected set;
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
        public event Action OnConnect = () => { };
        /// <summary>
        /// Occurs when [on disconnect].
        /// </summary>
        public event Action OnDisconnect = () => { };
        

        #endregion Other
    }
}