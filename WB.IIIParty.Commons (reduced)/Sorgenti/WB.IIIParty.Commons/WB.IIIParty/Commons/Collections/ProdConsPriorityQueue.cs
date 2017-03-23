// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-12-23 10:07:30 +0100 (lun, 23 dic 2013) $
//Versione: $Rev: 210 $
// ------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace WB.IIIParty.Commons.Collections
{
    #region Enum
    /// <summary>
    /// Enumerato del risultato di Enqueue
    /// </summary>
    public enum EnqueueResult { 
        /// <summary>
        /// L'oggetto è stato inserito correttamente nella coda.
        /// </summary>
        OK,
        /// <summary>
        /// Non è stato possibile inserire l'oggetto nella coda perchè ha raggiuntto il numero massimo di elementi.
        /// </summary>
        LISTFULL,
        /// <summary>
        /// Si è verificata un eccezione.
        /// </summary>
        EXCEPTION
    }

    #endregion
    /// <summary>
    /// Implementa una coda con produttori multipli e singolo consumatore
    /// </summary>
    public class ProdConsPriorityQueue<T> : IDisposable
    {

        #region Variable Declaration

        private int maxElement = int.MaxValue;
        private int countElement = 0;
        private bool orderPriorityCresc;
        private SortedDictionary<uint, Queue<T>> list = new SortedDictionary<uint, Queue<T>>();
        Semaphore m_Semaphore;

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_max_el">Numero massimo di elementi sulla coda</param>
        /// <param name="_orderPriorityCresc">Definisce se l'ordinamento della priorità è crescente o decrescente</param>
        public ProdConsPriorityQueue(bool _orderPriorityCresc, int _max_el)
        {
            this.maxElement = _max_el;
            this.orderPriorityCresc = _orderPriorityCresc;
            m_Semaphore = new Semaphore(0, this.maxElement);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Inserisce un oggetto sulla coda
        /// </summary>
        /// <param name="value">Oggetto da inserire sulla coda</param>
        /// <param name="priority">Priorità dell'oggetto</param>
        /// <returns>Ritorna l'esito dell'operazione</returns>
        public EnqueueResult Enqueue(uint priority, T value)
        {
            try
            {
                lock (this)
                {
                    if (this.countElement == this.maxElement)
                    {
                        return EnqueueResult.LISTFULL;
                    }
                    Queue<T> q;
                    if (!list.TryGetValue(priority, out q))
                    {
                        q = new Queue<T>();
                        list.Add(priority, q);

                    }
                    q.Enqueue(value);
                    this.countElement++;
                    
                    m_Semaphore.Release();

                    return EnqueueResult.OK;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return EnqueueResult.EXCEPTION;
            }
            finally
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns> 
        public bool Dequeue(out T obj)
        {
            obj = default(T);
            try
            {
                m_Semaphore.WaitOne();

                lock (this)
                {
                    if (this.orderPriorityCresc)
                    {
                        var pair = list.First();
                        obj = pair.Value.Dequeue();
                        if (pair.Value.Count == 0) // nothing left of the top priority.
                        {
                            list.Remove(pair.Key);
                            this.countElement--;
                        }
                    }
                    else
                    {
                        var pair = list.Last();
                        obj = pair.Value.Dequeue();
                        if (pair.Value.Count == 0) // nothing left of the top priority.
                        {
                            list.Remove(pair.Key);
                            this.countElement--;
                        }
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());                
            }
            finally
            {

            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T TryDequeue()
        {
            var pair = list.First();
            T v = pair.Value.First();
            return v;
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Ritorna se la coda è vuota
        /// </summary>
        public bool IsEmpty
        {
            get { return this.list.Count == 0; }
        }

        /// <summary>
        /// Ritorna il numero di elementi in coda
        /// </summary>
        public int QueueLength
        {
            get
            {
                return this.countElement;
            }
        }

        /// <summary>
        /// Ritorna il numero massimo di elementi in coda
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.maxElement;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Libera le risorse
        /// </summary>
        public void Dispose()
        {
            

        }

        #endregion
    }
}
