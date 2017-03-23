// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:14
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Serialization
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Class SerializableTimeSpan
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class SerializableTimeSpan
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSpan"/> class.
        /// </summary>
        public SerializableTimeSpan()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSpan"/> class.
        /// </summary>
        /// <param name="ts">The ts.</param>
        public SerializableTimeSpan(TimeSpan ts)
        {
            duration = (decimal) (ts.TotalSeconds + ts.TotalMilliseconds/1000);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        [XmlAttribute]
        private decimal duration
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs an implicit conversion from <see cref="TimeSpan"/> to <see cref="SerializableTimeSpan"/>.
        /// </summary>
        /// <param name="ts">The ts.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SerializableTimeSpan(TimeSpan ts)
        {
            return new SerializableTimeSpan(ts);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="SerializableTimeSpan"/> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="sts">The STS.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator TimeSpan(SerializableTimeSpan sts)
        {
            var ival = decimal.Floor(sts.duration);
            var fval = (sts.duration - ival)*1000M;
            return TimeSpan.FromSeconds((int) ival) + TimeSpan.FromMilliseconds((int) fval);
        }

        #endregion Methods
    }
}