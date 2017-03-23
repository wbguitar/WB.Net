// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:19
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Forms
{
    using System.Windows.Forms;

    /// <summary>
    /// Class AckNackForm
    /// </summary>
    public partial class AckNackForm : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AckNackForm"/> class.
        /// </summary>
        public AckNackForm()
        {
            InitializeComponent();
            btnAck.Click += (s, e)=>
                                {
                                    Res = Result.Ack;
                                    Close();
                                };
            btnNack.Click += (s, e)=>
                                 {
                                     Res = Result.Nack;
                                     Close();
                                 };
        }

        #endregion Constructors

        #region Enumerations

        /// <summary>
        /// Enum Result
        /// </summary>
        public enum Result
        {
            /// <summary>
            /// The ack
            /// </summary>
            Ack,
            /// <summary>
            /// The nack
            /// </summary>
            Nack
        }

        #endregion Enumerations

        #region Properties

        /// <summary>
        /// Gets the res.
        /// </summary>
        /// <value>The res.</value>
        public Result Res
        {
            get; private set;
        }

        #endregion Properties
    }
}