// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 09:21
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	WB.Commons.UI.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Observables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Windows.Threading;

    namespace BAK
    {
        /// <summary>
        /// Class ObservableDictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the T key.</typeparam>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        public class ObservableDictionary<TKey, TValue> : Serialization.SerializableDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            public ObservableDictionary()
            {
                Init();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="dict">The dict.</param>
            public ObservableDictionary(IDictionary<TKey, TValue> dict)
                : base(dict)
            {
                Init();
            }

            /// <summary>
            /// Gets or sets the dispatcher.
            /// </summary>
            /// <value>The dispatcher.</value>
            public Dispatcher Dispatcher
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the <see cref="`1"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>`1.</returns>
            public new TValue this[TKey key]
            {
                get { return (this as IDictionary<TKey, TValue>)[key]; }

                set
                {
                    NotifyCollectionChangedAction action;
                    if (Keys.Contains(key))
                    {
                        var oldval = new KeyValuePair<TKey, TValue>(key, this[key]);
                        var newval = new KeyValuePair<TKey, TValue>(key, value);
                        base[key] = value;
                        var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newval, oldval);
                        RaiseCollectionChanged(arg);
                    }
                    else
                    {
                        base[key] = value;
                        var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                                                       new KeyValuePair<TKey, TValue>(key, value));
                        RaiseCollectionChanged(arg);
                    }
                }
            }

            /// <summary>
            /// Occurs when the collection changes.
            /// </summary>
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Inits this instance.
            /// </summary>
            private void Init()
            {
                CollectionChanged += (s, e) => { };
                PropertyChanged += (s, e) => { };
                Dispatcher = null;
            }

            /// <summary>
            /// Raises the collection changed.
            /// </summary>
            /// <param name="arg">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
            public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs arg)
            {
                Dispatch(() =>
                {
                    CollectionChanged(this, arg);

                    RaisePropertyChanged("Item");
                    RaisePropertyChanged("Keys");
                    RaisePropertyChanged("Values");
                    RaisePropertyChanged("Count");
                });
            }

            /// <summary>
            /// Raises the property changed.
            /// </summary>
            /// <param name="property">The property.</param>
            public void RaisePropertyChanged(string property)
            {
                Dispatch(() =>
                {
                    var arg = new PropertyChangedEventArgs(property);
                    PropertyChanged(this, arg);
                });
            }

            /// <summary>
            /// Adds the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public new void Add(TKey key, TValue value)
            {
                Add(new KeyValuePair<TKey, TValue>(key, value));
            }

            /// <summary>
            /// Adds the specified pair.
            /// </summary>
            /// <param name="pair">The pair.</param>
            public void Add(KeyValuePair<TKey, TValue> pair)
            {
                Add(pair.Key, pair.Value);
                (this as IDictionary<TKey, TValue>).Add(pair);
                RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair));
            }

            /// <summary>
            /// Removes the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            public new void Remove(TKey key)
            {
                if (Keys.Contains(key))
                    Remove(key, this[key]);
            }

            /// <summary>
            /// Removes the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public void Remove(TKey key, TValue value)
            {
                Remove(new KeyValuePair<TKey, TValue>(key, value));
            }

            /// <summary>
            /// Removes the specified pair.
            /// </summary>
            /// <param name="pair">The pair.</param>
            public void Remove(KeyValuePair<TKey, TValue> pair)
            {
                if ((this as IDictionary<TKey, TValue>).Remove(pair))
                {
                    // vuole l'indice altrimenti si incazza, sempre = 0...
                    var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair, 0);
                    RaiseCollectionChanged(arg);
                }
            }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            public new void Clear()
            {
                base.Clear();
                RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// Dispatches the specified action.
            /// </summary>
            /// <param name="action">The action.</param>
            private void Dispatch(Action action)
            {
                if (Dispatcher != null)
                    Dispatcher.Invoke(action);
                else
                {
                    action();
                }
            }
        }
    }

    namespace BAK1
    {
        /// <summary>
        /// Class ObservableDictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the T key.</typeparam>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            /// <summary>
            /// The count string
            /// </summary>
            private const string CountString = "Count";

            /// <summary>
            /// The indexer name
            /// </summary>
            private const string IndexerName = "Item[]";

            /// <summary>
            /// The keys name
            /// </summary>
            private const string KeysName = "Keys";

            /// <summary>
            /// The values name
            /// </summary>
            private const string ValuesName = "Values";

            /// <summary>
            /// The _ dictionary
            /// </summary>
            private IDictionary<TKey, TValue> _Dictionary;

            /// <summary>
            /// Gets the dictionary.
            /// </summary>
            /// <value>The dictionary.</value>
            protected IDictionary<TKey, TValue> Dictionary
            {
                get { return _Dictionary; }
            }

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            public ObservableDictionary()
            {
                _Dictionary = new Dictionary<TKey, TValue>();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
            {
                _Dictionary = new Dictionary<TKey, TValue>(dictionary);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="comparer">The comparer.</param>
            public ObservableDictionary(IEqualityComparer<TKey> comparer)
            {
                _Dictionary = new Dictionary<TKey, TValue>(comparer);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="capacity">The capacity.</param>
            public ObservableDictionary(int capacity)
            {
                _Dictionary = new Dictionary<TKey, TValue>(capacity);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            /// <param name="comparer">The comparer.</param>
            public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            {
                _Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="capacity">The capacity.</param>
            /// <param name="comparer">The comparer.</param>
            public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            {
                _Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            }

            #endregion Constructors

            #region IDictionary<TKey,TValue> Members

            /// <summary>
            /// Adds the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public void Add(TKey key, TValue value)
            {
                Insert(key, value, true);
            }

            /// <summary>
            /// Determines whether the specified key contains key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
            public bool ContainsKey(TKey key)
            {
                return Dictionary.ContainsKey(key);
            }

            /// <summary>
            /// Gets the keys.
            /// </summary>
            /// <value>The keys.</value>
            public ICollection<TKey> Keys
            {
                get { return Dictionary.Keys; }
            }

            /// <summary>
            /// Removes the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            /// <exception cref="System.ArgumentNullException">key</exception>
            public bool Remove(TKey key)
            {
                if (key == null) throw new ArgumentNullException("key");

                TValue value;
                Dictionary.TryGetValue(key, out value);
                var removed = Dictionary.Remove(key);
                if (removed)
                    //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
                    OnCollectionChanged();
                return removed;
            }

            /// <summary>
            /// Tries the get value.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            public bool TryGetValue(TKey key, out TValue value)
            {
                return Dictionary.TryGetValue(key, out value);
            }

            /// <summary>
            /// Gets the values.
            /// </summary>
            /// <value>The values.</value>
            public ICollection<TValue> Values
            {
                get { return Dictionary.Values; }
            }

            /// <summary>
            /// Gets or sets the <see cref="`1"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>`1.</returns>
            public TValue this[TKey key]
            {
                get
                {
                    return Dictionary[key];
                }
                set
                {
                    Insert(key, value, false);
                }
            }

            #endregion IDictionary<TKey,TValue> Members

            #region ICollection<KeyValuePair<TKey,TValue>> Members

            /// <summary>
            /// Adds the specified item.
            /// </summary>
            /// <param name="item">The item.</param>
            public void Add(KeyValuePair<TKey, TValue> item)
            {
                Insert(item.Key, item.Value, true);
            }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            public void Clear()
            {
                if (Dictionary.Count > 0)
                {
                    Dictionary.Clear();
                    OnCollectionChanged();
                }
            }

            /// <summary>
            /// Determines whether [contains] [the specified item].
            /// </summary>
            /// <param name="item">The item.</param>
            /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return Dictionary.Contains(item);
            }

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                Dictionary.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            public int Count
            {
                get { return Dictionary.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public bool IsReadOnly
            {
                get { return Dictionary.IsReadOnly; }
            }

            /// <summary>
            /// Removes the specified item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return Remove(item.Key);
            }

            #endregion ICollection<KeyValuePair<TKey,TValue>> Members

            #region IEnumerable<KeyValuePair<TKey,TValue>> Members

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>IEnumerator{KeyValuePair{`0`1}}.</returns>
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return Dictionary.GetEnumerator();
            }

            #endregion IEnumerable<KeyValuePair<TKey,TValue>> Members

            #region IEnumerable Members

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)Dictionary).GetEnumerator();
            }

            #endregion IEnumerable Members

            #region INotifyCollectionChanged Members

            /// <summary>
            /// Occurs when the collection changes.
            /// </summary>
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            #endregion INotifyCollectionChanged Members

            #region INotifyPropertyChanged Members

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion INotifyPropertyChanged Members

            /// <summary>
            /// Adds the range.
            /// </summary>
            /// <param name="items">The items.</param>
            /// <exception cref="System.ArgumentNullException">items</exception>
            /// <exception cref="System.ArgumentException">An item with the same key has already been added.</exception>
            public void AddRange(IDictionary<TKey, TValue> items)
            {
                if (items == null) throw new ArgumentNullException("items");

                if (items.Count > 0)
                {
                    if (Dictionary.Count > 0)
                    {
                        if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                            throw new ArgumentException("An item with the same key has already been added.");
                        else
                            foreach (var item in items) Dictionary.Add(item);
                    }
                    else
                        _Dictionary = new Dictionary<TKey, TValue>(items);

                    OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
                }
            }

            /// <summary>
            /// Inserts the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <param name="add">if set to <c>true</c> [add].</param>
            /// <exception cref="System.ArgumentNullException">key</exception>
            /// <exception cref="System.ArgumentException">An item with the same key has already been added.</exception>
            private void Insert(TKey key, TValue value, bool add)
            {
                if (key == null) throw new ArgumentNullException("key");

                TValue item;
                if (Dictionary.TryGetValue(key, out item))
                {
                    if (add) throw new ArgumentException("An item with the same key has already been added.");
                    if (Equals(item, value)) return;
                    Dictionary[key] = value;

                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
                }
                else
                {
                    Dictionary[key] = value;

                    OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
                }
            }

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            private void OnPropertyChanged()
            {
                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnPropertyChanged(KeysName);
                OnPropertyChanged(ValuesName);
            }

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            /// <summary>
            /// Called when [collection changed].
            /// </summary>
            private void OnCollectionChanged()
            {
                OnPropertyChanged();
                if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// Called when [collection changed].
            /// </summary>
            /// <param name="action">The action.</param>
            /// <param name="changedItem">The changed item.</param>
            private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
            {
                OnPropertyChanged();
                if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }

            /// <summary>
            /// Called when [collection changed].
            /// </summary>
            /// <param name="action">The action.</param>
            /// <param name="newItem">The new item.</param>
            /// <param name="oldItem">The old item.</param>
            private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
            {
                OnPropertyChanged();
                if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
            }

            /// <summary>
            /// Called when [collection changed].
            /// </summary>
            /// <param name="action">The action.</param>
            /// <param name="newItems">The new items.</param>
            private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
            {
                OnPropertyChanged();
                if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
            }
        }
    }

    namespace BAK2
    {
        /// <summary>
        /// Class ObservableDictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the T key.</typeparam>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        [Serializable]
        public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable, ISerializable, IDeserializationCallback, INotifyCollectionChanged, INotifyPropertyChanged
        {
            #region constructors

            #region public

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            public ObservableDictionary()
            {
                _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
            {
                _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();

                foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                    DoAddEntry((TKey)entry.Key, (TValue)entry.Value);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="comparer">The comparer.</param>
            public ObservableDictionary(IEqualityComparer<TKey> comparer)
            {
                _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            /// <param name="comparer">The comparer.</param>
            public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            {
                _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);

                foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                    DoAddEntry((TKey)entry.Key, (TValue)entry.Value);
            }

            #endregion public

            #region protected

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="info">The info.</param>
            /// <param name="context">The context.</param>
            protected ObservableDictionary(SerializationInfo info, StreamingContext context)
            {
                _siInfo = info;
            }

            #endregion protected

            #endregion constructors

            #region properties

            #region public

            /// <summary>
            /// Gets the comparer.
            /// </summary>
            /// <value>The comparer.</value>
            public IEqualityComparer<TKey> Comparer
            {
                get { return _keyedEntryCollection.Comparer; }
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            public int Count
            {
                get { return _keyedEntryCollection.Count; }
            }

            /// <summary>
            /// Gets the keys.
            /// </summary>
            /// <value>The keys.</value>
            public Dictionary<TKey, TValue>.KeyCollection Keys
            {
                get { return TrueDictionary.Keys; }
            }

            /// <summary>
            /// Gets or sets the <see cref="`1"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>`1.</returns>
            public TValue this[TKey key]
            {
                get { return (TValue)_keyedEntryCollection[key].Value; }
                set { DoSetEntry(key, value); }
            }

            /// <summary>
            /// Gets the values.
            /// </summary>
            /// <value>The values.</value>
            public Dictionary<TKey, TValue>.ValueCollection Values
            {
                get { return TrueDictionary.Values; }
            }

            #endregion public

            #region private

            /// <summary>
            /// Gets the true dictionary.
            /// </summary>
            /// <value>The true dictionary.</value>
            private Dictionary<TKey, TValue> TrueDictionary
            {
                get
                {
                    if (_dictionaryCacheVersion != _version)
                    {
                        _dictionaryCache.Clear();
                        foreach (DictionaryEntry entry in _keyedEntryCollection)
                            _dictionaryCache.Add((TKey)entry.Key, (TValue)entry.Value);
                        _dictionaryCacheVersion = _version;
                    }
                    return _dictionaryCache;
                }
            }

            #endregion private

            #endregion properties

            #region methods

            #region public

            /// <summary>
            /// Adds the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public void Add(TKey key, TValue value)
            {
                DoAddEntry(key, value);
            }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            public void Clear()
            {
                DoClearEntries();
            }

            /// <summary>
            /// Determines whether the specified key contains key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
            public bool ContainsKey(TKey key)
            {
                return _keyedEntryCollection.Contains(key);
            }

            /// <summary>
            /// Determines whether the specified value contains value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if the specified value contains value; otherwise, <c>false</c>.</returns>
            public bool ContainsValue(TValue value)
            {
                return TrueDictionary.ContainsValue(value);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            public IEnumerator GetEnumerator()
            {
                return new Enumerator<TKey, TValue>(this, false);
            }

            /// <summary>
            /// Removes the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            public bool Remove(TKey key)
            {
                return DoRemoveEntry(key);
            }

            /// <summary>
            /// Tries the get value.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            public bool TryGetValue(TKey key, out TValue value)
            {
                bool result = _keyedEntryCollection.Contains(key);
                value = result ? (TValue)_keyedEntryCollection[key].Value : default(TValue);
                return result;
            }

            #endregion public

            #region protected

            /// <summary>
            /// Adds the entry.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            protected virtual bool AddEntry(TKey key, TValue value)
            {
                _keyedEntryCollection.Add(new DictionaryEntry(key, value));
                return true;
            }

            /// <summary>
            /// Clears the entries.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            protected virtual bool ClearEntries()
            {
                // check whether there are entries to clear
                bool result = (Count > 0);
                if (result)
                {
                    // if so, clear the dictionary
                    _keyedEntryCollection.Clear();
                }
                return result;
            }

            /// <summary>
            /// Gets the index and entry for key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="entry">The entry.</param>
            /// <returns>System.Int32.</returns>
            protected int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
            {
                entry = new DictionaryEntry();
                int index = -1;
                if (_keyedEntryCollection.Contains(key))
                {
                    entry = _keyedEntryCollection[key];
                    index = _keyedEntryCollection.IndexOf(entry);
                }
                return index;
            }

            /// <summary>
            /// Raises the <see cref="E:CollectionChanged" /> event.
            /// </summary>
            /// <param name="args">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
            {
                if (CollectionChanged != null)
                    CollectionChanged(this, args);
            }

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="name">The name.</param>
            protected virtual void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            /// <summary>
            /// Removes the entry.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            protected virtual bool RemoveEntry(TKey key)
            {
                // remove the entry
                return _keyedEntryCollection.Remove(key);
            }

            /// <summary>
            /// Sets the entry.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            protected virtual bool SetEntry(TKey key, TValue value)
            {
                bool keyExists = _keyedEntryCollection.Contains(key);

                // if identical key/value pair already exists, nothing to do
                if (keyExists && value.Equals((TValue)_keyedEntryCollection[key].Value))
                    return false;

                // otherwise, remove the existing entry
                if (keyExists)
                    _keyedEntryCollection.Remove(key);

                // add the new entry
                _keyedEntryCollection.Add(new DictionaryEntry(key, value));

                return true;
            }

            #endregion protected

            #region private

            /// <summary>
            /// Does the add entry.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            private void DoAddEntry(TKey key, TValue value)
            {
                if (AddEntry(key, value))
                {
                    _version++;

                    DictionaryEntry entry;
                    int index = GetIndexAndEntryForKey(key, out entry);
                    FireEntryAddedNotifications(entry, index);
                }
            }

            /// <summary>
            /// Does the clear entries.
            /// </summary>
            private void DoClearEntries()
            {
                if (ClearEntries())
                {
                    _version++;
                    FireResetNotifications();
                }
            }

            /// <summary>
            /// Does the remove entry.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            private bool DoRemoveEntry(TKey key)
            {
                DictionaryEntry entry;
                int index = GetIndexAndEntryForKey(key, out entry);

                bool result = RemoveEntry(key);
                if (result)
                {
                    _version++;
                    if (index > -1)
                        FireEntryRemovedNotifications(entry, index);
                }

                return result;
            }

            /// <summary>
            /// Does the set entry.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            private void DoSetEntry(TKey key, TValue value)
            {
                DictionaryEntry entry;
                int index = GetIndexAndEntryForKey(key, out entry);

                if (SetEntry(key, value))
                {
                    _version++;

                    // if prior entry existed for this key, fire the removed notifications
                    if (index > -1)
                    {
                        FireEntryRemovedNotifications(entry, index);

                        // force the property change notifications to fire for the modified entry
                        _countCache--;
                    }

                    // then fire the added notifications
                    index = GetIndexAndEntryForKey(key, out entry);
                    FireEntryAddedNotifications(entry, index);
                }
            }

            /// <summary>
            /// Fires the entry added notifications.
            /// </summary>
            /// <param name="entry">The entry.</param>
            /// <param name="index">The index.</param>
            private void FireEntryAddedNotifications(DictionaryEntry entry, int index)
            {
                // fire the relevant PropertyChanged notifications
                FirePropertyChangedNotifications();

                // fire CollectionChanged notification
                if (index > -1)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value), index));
                else
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// Fires the entry removed notifications.
            /// </summary>
            /// <param name="entry">The entry.</param>
            /// <param name="index">The index.</param>
            private void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
            {
                // fire the relevant PropertyChanged notifications
                FirePropertyChangedNotifications();

                // fire CollectionChanged notification
                if (index > -1)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value), index));
                else
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// Fires the property changed notifications.
            /// </summary>
            private void FirePropertyChangedNotifications()
            {
                if (Count != _countCache)
                {
                    _countCache = Count;
                    OnPropertyChanged("Count");
                    OnPropertyChanged("Item[]");
                    OnPropertyChanged("Keys");
                    OnPropertyChanged("Values");
                }
            }

            /// <summary>
            /// Fires the reset notifications.
            /// </summary>
            private void FireResetNotifications()
            {
                // fire the relevant PropertyChanged notifications
                FirePropertyChangedNotifications();

                // fire CollectionChanged notification
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            #endregion private

            #endregion methods

            #region interfaces

            #region IDictionary<TKey, TValue>

            /// <summary>
            /// Adds the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
            {
                DoAddEntry(key, value);
            }

            /// <summary>
            /// Removes the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            bool IDictionary<TKey, TValue>.Remove(TKey key)
            {
                return DoRemoveEntry(key);
            }

            /// <summary>
            /// Determines whether the specified key contains key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
            bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
            {
                return _keyedEntryCollection.Contains(key);
            }

            /// <summary>
            /// Tries the get value.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
            {
                return TryGetValue(key, out value);
            }

            /// <summary>
            /// Gets the keys.
            /// </summary>
            /// <value>The keys.</value>
            ICollection<TKey> IDictionary<TKey, TValue>.Keys
            {
                get { return Keys; }
            }

            /// <summary>
            /// Gets the values.
            /// </summary>
            /// <value>The values.</value>
            ICollection<TValue> IDictionary<TKey, TValue>.Values
            {
                get { return Values; }
            }

            /// <summary>
            /// Gets or sets the <see cref="`1"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>`1.</returns>
            TValue IDictionary<TKey, TValue>.this[TKey key]
            {
                get { return (TValue)_keyedEntryCollection[key].Value; }
                set { DoSetEntry(key, value); }
            }

            #endregion IDictionary<TKey, TValue>

            #region IDictionary

            /// <summary>
            /// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
            /// </summary>
            /// <param name="key">The <see cref="T:System.Object" /> to use as the key of the element to add.</param>
            /// <param name="value">The <see cref="T:System.Object" /> to use as the value of the element to add.</param>
            void IDictionary.Add(object key, object value)
            {
                DoAddEntry((TKey)key, (TValue)value);
            }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            void IDictionary.Clear()
            {
                DoClearEntries();
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.
            /// </summary>
            /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.</param>
            /// <returns>true if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.</returns>
            bool IDictionary.Contains(object key)
            {
                return _keyedEntryCollection.Contains((TKey)key);
            }

            /// <summary>
            /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.</returns>
            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return new Enumerator<TKey, TValue>(this, true);
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.
            /// </summary>
            /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
            /// <returns>true if the <see cref="T:System.Collections.IDictionary" /> object has a fixed size; otherwise, false.</returns>
            bool IDictionary.IsFixedSize
            {
                get { return false; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            bool IDictionary.IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Gets or sets the <see cref="System.Object"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>System.Object.</returns>
            object IDictionary.this[object key]
            {
                get { return _keyedEntryCollection[(TKey)key].Value; }
                set { DoSetEntry((TKey)key, (TValue)value); }
            }

            /// <summary>
            /// Gets the keys.
            /// </summary>
            /// <value>The keys.</value>
            ICollection IDictionary.Keys
            {
                get { return Keys; }
            }

            /// <summary>
            /// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            void IDictionary.Remove(object key)
            {
                DoRemoveEntry((TKey)key);
            }

            /// <summary>
            /// Gets the values.
            /// </summary>
            /// <value>The values.</value>
            ICollection IDictionary.Values
            {
                get { return Values; }
            }

            #endregion IDictionary

            #region ICollection<KeyValuePair<TKey, TValue>>

            /// <summary>
            /// Adds the specified KVP.
            /// </summary>
            /// <param name="kvp">The KVP.</param>
            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> kvp)
            {
                DoAddEntry(kvp.Key, kvp.Value);
            }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            void ICollection<KeyValuePair<TKey, TValue>>.Clear()
            {
                DoClearEntries();
            }

            /// <summary>
            /// Determines whether [contains] [the specified KVP].
            /// </summary>
            /// <param name="kvp">The KVP.</param>
            /// <returns><c>true</c> if [contains] [the specified KVP]; otherwise, <c>false</c>.</returns>
            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> kvp)
            {
                return _keyedEntryCollection.Contains(kvp.Key);
            }

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="index">The index.</param>
            /// <exception cref="System.ArgumentNullException">CopyTo() failed:  array parameter was null</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">CopyTo() failed:  index parameter was outside the bounds of the supplied array</exception>
            /// <exception cref="System.ArgumentException">CopyTo() failed:  supplied array was too small</exception>
            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("CopyTo() failed:  array parameter was null");
                }
                if ((index < 0) || (index > array.Length))
                {
                    throw new ArgumentOutOfRangeException("CopyTo() failed:  index parameter was outside the bounds of the supplied array");
                }
                if ((array.Length - index) < _keyedEntryCollection.Count)
                {
                    throw new ArgumentException("CopyTo() failed:  supplied array was too small");
                }

                foreach (DictionaryEntry entry in _keyedEntryCollection)
                    array[index++] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            int ICollection<KeyValuePair<TKey, TValue>>.Count
            {
                get { return _keyedEntryCollection.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes the specified KVP.
            /// </summary>
            /// <param name="kvp">The KVP.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> kvp)
            {
                return DoRemoveEntry(kvp.Key);
            }

            #endregion ICollection<KeyValuePair<TKey, TValue>>

            #region ICollection

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_keyedEntryCollection).CopyTo(array, index);
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            int ICollection.Count
            {
                get { return _keyedEntryCollection.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
            /// </summary>
            /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
            /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.</returns>
            bool ICollection.IsSynchronized
            {
                get { return ((ICollection)_keyedEntryCollection).IsSynchronized; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
            /// </summary>
            /// <value>The sync root.</value>
            /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
            object ICollection.SyncRoot
            {
                get { return ((ICollection)_keyedEntryCollection).SyncRoot; }
            }

            #endregion ICollection

            #region IEnumerable<KeyValuePair<TKey, TValue>>

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>IEnumerator{KeyValuePair{`0`1}}.</returns>
            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return new Enumerator<TKey, TValue>(this, false);
            }

            #endregion IEnumerable<KeyValuePair<TKey, TValue>>

            #region IEnumerable

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion IEnumerable

            #region ISerializable

            /// <summary>
            /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
            /// </summary>
            /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
            /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
            /// <exception cref="System.ArgumentNullException">info</exception>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info");
                }

                Collection<DictionaryEntry> entries = new Collection<DictionaryEntry>();
                foreach (DictionaryEntry entry in _keyedEntryCollection)
                    entries.Add(entry);
                info.AddValue("entries", entries);
            }

            #endregion ISerializable

            #region IDeserializationCallback

            /// <summary>
            /// Runs when the entire object graph has been deserialized.
            /// </summary>
            /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
            public virtual void OnDeserialization(object sender)
            {
                if (_siInfo != null)
                {
                    Collection<DictionaryEntry> entries = (Collection<DictionaryEntry>)
                        _siInfo.GetValue("entries", typeof(Collection<DictionaryEntry>));
                    foreach (DictionaryEntry entry in entries)
                        AddEntry((TKey)entry.Key, (TValue)entry.Value);
                }
            }

            #endregion IDeserializationCallback

            #region INotifyCollectionChanged

            /// <summary>
            /// Occurs when the collection changes.
            /// </summary>
            event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
            {
                add { CollectionChanged += value; }
                remove { CollectionChanged -= value; }
            }

            /// <summary>
            /// Occurs when the collection changes.
            /// </summary>
            protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

            #endregion INotifyCollectionChanged

            #region INotifyPropertyChanged

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { PropertyChanged += value; }
                remove { PropertyChanged -= value; }
            }

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            protected virtual event PropertyChangedEventHandler PropertyChanged;

            #endregion INotifyPropertyChanged

            #endregion interfaces

            #region protected classes

            #region KeyedDictionaryEntryCollection<TKey>

            /// <summary>
            /// Class KeyedDictionaryEntryCollection
            /// </summary>
            /// <typeparam name="TKey">The type of the T key.</typeparam>
            protected class KeyedDictionaryEntryCollection<TKey> : KeyedCollection<TKey, DictionaryEntry>
            {
                #region constructors

                #region public

                /// <summary>
                /// Initializes a new instance of the <see cref="KeyedDictionaryEntryCollection`1"/> class.
                /// </summary>
                public KeyedDictionaryEntryCollection()
                    : base()
                {
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="KeyedDictionaryEntryCollection`1"/> class.
                /// </summary>
                /// <param name="comparer">The comparer.</param>
                public KeyedDictionaryEntryCollection(IEqualityComparer<TKey> comparer)
                    : base(comparer)
                {
                }

                #endregion public

                #endregion constructors

                #region methods

                #region protected

                /// <summary>
                /// Gets the key for item.
                /// </summary>
                /// <param name="entry">The entry.</param>
                /// <returns>`0.</returns>
                protected override TKey GetKeyForItem(DictionaryEntry entry)
                {
                    return (TKey)entry.Key;
                }

                #endregion protected

                #endregion methods
            }

            #endregion KeyedDictionaryEntryCollection<TKey>

            #endregion protected classes

            #region public structures

            #region Enumerator

            /// <summary>
            /// Struct Enumerator
            /// </summary>
            /// <typeparam name="TKey">The type of the T key.</typeparam>
            /// <typeparam name="TValue">The type of the T value.</typeparam>
            [Serializable,
            StructLayout(LayoutKind.Sequential)]
            public struct Enumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
            {
                #region constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerator`2"/> struct.
                /// </summary>
                /// <param name="dictionary">The dictionary.</param>
                /// <param name="isDictionaryEntryEnumerator">if set to <c>true</c> [is dictionary entry enumerator].</param>
                internal Enumerator(ObservableDictionary<TKey, TValue> dictionary, bool isDictionaryEntryEnumerator)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = -1;
                    _isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                    _current = new KeyValuePair<TKey, TValue>();
                }

                #endregion constructors

                #region properties

                #region public

                /// <summary>
                /// Gets the current.
                /// </summary>
                /// <value>The current.</value>
                public KeyValuePair<TKey, TValue> Current
                {
                    get
                    {
                        ValidateCurrent();
                        return _current;
                    }
                }

                #endregion public

                #endregion properties

                #region methods

                #region public

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// Advances the enumerator to the next element of the collection.
                /// </summary>
                /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    ValidateVersion();
                    _index++;
                    if (_index < _dictionary._keyedEntryCollection.Count)
                    {
                        _current = new KeyValuePair<TKey, TValue>((TKey)_dictionary._keyedEntryCollection[_index].Key, (TValue)_dictionary._keyedEntryCollection[_index].Value);
                        return true;
                    }
                    _index = -2;
                    _current = new KeyValuePair<TKey, TValue>();
                    return false;
                }

                #endregion public

                #region private

                /// <summary>
                /// Validates the current.
                /// </summary>
                /// <exception cref="System.InvalidOperationException">
                /// The enumerator has not been started.
                /// or
                /// The enumerator has reached the end of the collection.
                /// </exception>
                private void ValidateCurrent()
                {
                    if (_index == -1)
                    {
                        throw new InvalidOperationException("The enumerator has not been started.");
                    }
                    else if (_index == -2)
                    {
                        throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                    }
                }

                /// <summary>
                /// Validates the version.
                /// </summary>
                /// <exception cref="System.InvalidOperationException">The enumerator is not valid because the dictionary changed.</exception>
                private void ValidateVersion()
                {
                    if (_version != _dictionary._version)
                    {
                        throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                    }
                }

                #endregion private

                #endregion methods

                #region IEnumerator implementation

                /// <summary>
                /// Gets the current.
                /// </summary>
                /// <value>The current.</value>
                object IEnumerator.Current
                {
                    get
                    {
                        ValidateCurrent();
                        if (_isDictionaryEntryEnumerator)
                        {
                            return new DictionaryEntry(_current.Key, _current.Value);
                        }
                        return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
                    }
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                void IEnumerator.Reset()
                {
                    ValidateVersion();
                    _index = -1;
                    _current = new KeyValuePair<TKey, TValue>();
                }

                #endregion IEnumerator implementation

                #region IDictionaryEnumerator implemenation

                /// <summary>
                /// Gets the entry.
                /// </summary>
                /// <value>The entry.</value>
                DictionaryEntry IDictionaryEnumerator.Entry
                {
                    get
                    {
                        ValidateCurrent();
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                }

                /// <summary>
                /// Gets the key.
                /// </summary>
                /// <value>The key.</value>
                object IDictionaryEnumerator.Key
                {
                    get
                    {
                        ValidateCurrent();
                        return _current.Key;
                    }
                }

                /// <summary>
                /// Gets the value.
                /// </summary>
                /// <value>The value.</value>
                object IDictionaryEnumerator.Value
                {
                    get
                    {
                        ValidateCurrent();
                        return _current.Value;
                    }
                }

                #endregion IDictionaryEnumerator implemenation

                #region fields

                /// <summary>
                /// The _dictionary
                /// </summary>
                private ObservableDictionary<TKey, TValue> _dictionary;

                /// <summary>
                /// The _version
                /// </summary>
                private int _version;

                /// <summary>
                /// The _index
                /// </summary>
                private int _index;

                /// <summary>
                /// The _current
                /// </summary>
                private KeyValuePair<TKey, TValue> _current;

                /// <summary>
                /// The _is dictionary entry enumerator
                /// </summary>
                private bool _isDictionaryEntryEnumerator;

                #endregion fields
            }

            #endregion Enumerator

            #endregion public structures

            #region fields

            /// <summary>
            /// The _keyed entry collection
            /// </summary>
            protected KeyedDictionaryEntryCollection<TKey> _keyedEntryCollection;

            /// <summary>
            /// The _count cache
            /// </summary>
            private int _countCache = 0;

            /// <summary>
            /// The _dictionary cache
            /// </summary>
            private Dictionary<TKey, TValue> _dictionaryCache = new Dictionary<TKey, TValue>();

            /// <summary>
            /// The _dictionary cache version
            /// </summary>
            private int _dictionaryCacheVersion = 0;

            /// <summary>
            /// The _version
            /// </summary>
            private int _version = 0;

            /// <summary>
            /// The _si info
            /// </summary>
            [NonSerialized]
            private SerializationInfo _siInfo = null;

            #endregion fields
        }
    }

    /// <summary>
    /// Class ObservableDictionary
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TValue">The type of the T value.</typeparam>
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        #region Delegates

        /// <summary>
        /// Delegate KeyAddedEvent
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public delegate void KeyAddedEvent(object sender, KeyAddedEventArgs<TKey, TValue> e);

        /// <summary>
        /// Delegate KeyModifiedEvent
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public delegate void KeyModifiedEvent(object sender, KeyModifiedEventArgs<TKey, TValue> e);

        /// <summary>
        /// Delegate KeyRemovedEvent
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public delegate void KeyRemovedEvent(object sender, KeyEventArgs<TKey> e);

        /// <summary>
        /// Delegate KeyRetrievedEvent
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public delegate void KeyRetrievedEvent(object sender, KeyEventArgs<TKey> e);

        /// <summary>
        /// Delegate ListClearedEvent
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public delegate void ListClearedEvent(object sender, EventArgs e);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [key added].
        /// </summary>
        public event KeyAddedEvent KeyAdded;

        /// <summary>
        /// Occurs when [key modified].
        /// </summary>
        public event KeyModifiedEvent KeyModified;

        /// <summary>
        /// Occurs when [key removed].
        /// </summary>
        public event KeyRemovedEvent KeyRemoved;

        /// <summary>
        /// Occurs when [key retrieved].
        /// </summary>
        public event KeyRetrievedEvent KeyRetrieved;

        /// <summary>
        /// Occurs when [list cleared].
        /// </summary>
        public event ListClearedEvent ListCleared;

        #endregion Events

        #region Indexers

        /// <summary>
        /// Gets or sets the <see cref="`1"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>`1.</returns>
        public new TValue this[TKey key]
        {
            get
            {
                if (KeyRetrieved != null)
                    KeyRetrieved(this, new KeyEventArgs<TKey> { Key = key });
                return base[key];

            }
            set
            {
                //if the key is not in the list add it
                //this mocks up the default behavior, but forces it to go through
                //the new Add function which raises the event.
                if (!ContainsKey(key))
                    Add(key, value);

                TValue prevValue = base[key];
                if (!prevValue.Equals(value))
                {
                    base[key] = value;
                    if (KeyModified != null)
                        KeyModified(this, new KeyModifiedEventArgs<TKey, TValue> { Key = key, NewValue = value, PreviousValue = prevValue });
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            if (KeyAdded != null)
                KeyAdded(this, new KeyAddedEventArgs<TKey, TValue> { Key = key, Value = value });
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public new void Clear()
        {
            if (Keys.Count > 0)
            {
                base.Clear();
                if (ListCleared != null)
                    ListCleared(this, new EventArgs());
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public new bool Remove(TKey key)
        {
            bool retValue = base.Remove(key);
            if (retValue && KeyRemoved != null)
                KeyRemoved(this, new KeyEventArgs<TKey> { Key = key });

            return retValue;
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Class KeyAddedEventArgs
        /// </summary>
        /// <typeparam name="TKey">The type of the T key.</typeparam>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        public class KeyAddedEventArgs<TKey, TValue>
        {
            #region Properties

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            /// <value>The key.</value>
            public TKey Key
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public TValue Value
            {
                get; set;
            }

            #endregion Properties
        }

        /// <summary>
        /// Class KeyEventArgs
        /// </summary>
        /// <typeparam name="TKey">The type of the T key.</typeparam>
        public class KeyEventArgs<TKey>
        {
            #region Properties

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            /// <value>The key.</value>
            public TKey Key
            {
                get; set;
            }

            #endregion Properties
        }

        /// <summary>
        /// Class KeyModifiedEventArgs
        /// </summary>
        /// <typeparam name="TKey">The type of the T key.</typeparam>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        public class KeyModifiedEventArgs<TKey, TValue>
        {
            #region Properties

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            /// <value>The key.</value>
            public TKey Key
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the new value.
            /// </summary>
            /// <value>The new value.</value>
            public TValue NewValue
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the previous value.
            /// </summary>
            /// <value>The previous value.</value>
            public TValue PreviousValue
            {
                get; set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}