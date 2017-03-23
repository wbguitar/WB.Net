// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:18
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Forms
{
    using System.Windows.Forms;

    // WPF control
    /// <summary>
    /// Windows Forms Control that can contain a generic WPF control
    /// </summary>
    /// <typeparam name="T">The type of the wpf control to contain</typeparam>
    public partial class WpfControlContainer<T> : UserControl
        where T : System.Windows.UIElement
    {
        #region Fields

        /// <summary>
        /// The _WPF control
        /// </summary>
        private T _wpfControl;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfControlContainer{T}"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public WpfControlContainer(T control)
        {
            InitializeComponent();

            _wpfControl = control;
            elementHost.Child = control;

            Dock = DockStyle.Fill;
        }

        #endregion Constructors
    }
}