// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:19
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Forms
{
    /// <summary>
    /// Struct ComboBoxItem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ComboBoxItem<T>
    {
        #region Fields

        /// <summary>
        /// The _value
        /// </summary>
        private readonly T _value;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxItem{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public ComboBoxItem(T value)
            :this()
        {
            _value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return _value; }
        }

        public string Text;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Text) ? Value.ToString() : Text;
        }

        #endregion Methods
    }
}