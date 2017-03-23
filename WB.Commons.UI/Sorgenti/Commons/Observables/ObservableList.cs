// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:20
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Observables
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class ObservableList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservableList<T> : List<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        public ObservableList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public ObservableList(IEnumerable<T> items)
            : base(items)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public ObservableList(int capacity)
            : base(capacity)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public new void Add(T obj)
        {
            base.Add(obj);
            OnCollectionChanged();
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public new void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);
            OnCollectionChanged();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged();
        }

        /// <summary>
        /// Inserts the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="item">The item.</param>
        public new void Insert(int i, T item)
        {
            base.Insert(i, item);
            OnCollectionChanged();
        }

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="items">The items.</param>
        public new void InsertRange(int i, IEnumerable<T> items)
        {
            base.InsertRange(i, items);
            OnCollectionChanged();
        }

        /// <summary>
        /// Removes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public new bool Remove(T obj)
        {
            var ok = base.Remove(obj);
            if (ok)
                OnCollectionChanged();

            return ok;
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="pred">The pred.</param>
        /// <returns>System.Int32.</returns>
        public new int RemoveAll(Predicate<T> pred)
        {
            var count = base.RemoveAll(pred);
            OnCollectionChanged();
            return count;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="i">The i.</param>
        public new void RemoveAt(int i)
        {
            base.RemoveAt(i);
            OnCollectionChanged();
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="count">The count.</param>
        public new void RemoveRange(int i, int count)
        {
            base.RemoveRange(i, count);
            OnCollectionChanged();
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Occurs when [on collection changed].
        /// </summary>
        public event Action OnCollectionChanged = () => { };
        

        #endregion Other
    }
}