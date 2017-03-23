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
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Struct SerializablePair
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TValue">The type of the T value.</typeparam>
    [Serializable]
    [XmlRoot]
    public struct SerializablePair<TKey, TValue>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializablePair{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="k">The k.</param>
        /// <param name="v">The v.</param>
        public SerializablePair(TKey k, TValue v)
            : this()
        {
            Key = k;
            Value = v;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [XmlElement]
        public TKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlElement]
        public TValue Value
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs an implicit conversion from <see cref="SerializablePair{"/> to <see cref="KeyValuePair{`0`1}"/>.
        /// </summary>
        /// <param name="sp">The sp.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator KeyValuePair<TKey, TValue>(SerializablePair<TKey, TValue> sp)
        {
            return new KeyValuePair<TKey, TValue>(sp.Key, sp.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var sp = (SerializablePair<TKey, TValue>) obj;

            return (Key.Equals(sp.Key)) && (Value.Equals(sp.Value));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Key: {0}, Value: {1}", Key, Value);
        }

        #endregion Methods
    }
}