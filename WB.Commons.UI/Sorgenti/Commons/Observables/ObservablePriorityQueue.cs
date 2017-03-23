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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Threading;

    /// <summary>
    /// Class ObservablePriorityQueue
    /// </summary>
    /// <typeparam name="TValue">The type of the T value.</typeparam>
    /// <typeparam name="TPriority">The type of the T priority.</typeparam>
    public class ObservablePriorityQueue<TValue, TPriority> : IEnumerable<TValue>, INotifyCollectionChanged
        where TValue : INotifyPropertyChanged
        where TPriority : IComparable
    {
        #region Fields

        /// <summary>
        /// The count
        /// </summary>
        private int count;

        /// <summary>
        /// The dict
        /// </summary>
        private SortedDictionary<TPriority, Queue<TValue>> dict;

        /// <summary>
        /// The dispatcher
        /// </summary>
        private Dispatcher dispatcher;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservablePriorityQueue{TValue, TPriority}"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public ObservablePriorityQueue(Dispatcher dispatcher = null)
        {
            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
            this.count = 0;
            this.dict = new SortedDictionary<TPriority, Queue<TValue>>(new ReverseComparer());
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ObservablePriorityQueue{TValue, TPriority}"/> is empty.
        /// </summary>
        /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
        public bool Empty
        {
            get { return Count == 0; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator{`0}.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var queue in dict.Values)
            {
                foreach (var value in queue)
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns>`0.</returns>
        public virtual TValue Peek()
        {
            return dict.First().Value.Peek();
        }

        /// <summary>
        /// Pops this instance.
        /// </summary>
        /// <returns>`0.</returns>
        public virtual TValue Pop()
        {
            if (dispatcher.CheckAccess())
            {
                --count;
                var pair = dict.First();
                var queue = pair.Value;
                var val = queue.Dequeue();
                if (queue.Count == 0) dict.Remove(pair.Key);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, val));
                return val;
            }
            else
            {
                return (TValue)dispatcher.Invoke(new Func<TValue>(Pop), DispatcherPriority.Send);
            }
        }

        /// <summary>
        /// Pushes the specified val.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="pri">The pri.</param>
        public virtual void Push(TValue val, TPriority pri = default(TPriority))
        {
            if (dispatcher.CheckAccess())
            {
                ++count;
                if (!dict.ContainsKey(pri)) dict[pri] = new Queue<TValue>();
                dict[pri].Enqueue(val);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, val));
            }
            else
            {
                dispatcher.Invoke(new Action<TValue, TPriority>(Push), DispatcherPriority.Send, val, pri);
            }
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Class ReverseComparer
        /// </summary>
        private class ReverseComparer : IComparer<TPriority>
        {
            #region Methods

            /// <summary>
            /// Compares the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>System.Int32.</returns>
            public int Compare(TPriority x, TPriority y)
            {
                return y.CompareTo(x);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}