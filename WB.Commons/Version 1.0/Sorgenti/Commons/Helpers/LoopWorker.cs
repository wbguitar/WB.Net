// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:17
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Esegue a loop un'attività asincrona
    /// </summary>
    public class LoopWorker : IDisposable
    {
        #region Events
        /// <summary>
        /// Occurs when [on disposed].
        /// </summary>
        public event Action OnDisposed = () => { };

        /// <summary>
        /// Occurs when [on loop].
        /// </summary>
        public event Action OnLoop = () => { };
        #endregion

        #region Fields

        /// <summary>
        /// The gc thread
        /// </summary>
        private Thread gcThread;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopWorker" /> class.
        /// </summary>
        /// <param name="polling">The polling.</param>
        public LoopWorker(TimeSpan polling)
        {
            Polling = polling;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get; protected set;
        }

        /// <summary>
        /// Gets or sets the polling.
        /// </summary>
        /// <value>The polling.</value>
        public TimeSpan Polling
        {
            get; protected set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            OnDisposed();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Start(TimeSpan.Zero);
        }

        /// <summary>
        /// Starts the specified polling.
        /// </summary>
        /// <param name="polling">The polling.</param>
        public void Start(TimeSpan polling)
        {
            if (gcThread != null && gcThread.IsAlive)
            {
                Stop();
                gcThread = null;
            }

            if (polling != TimeSpan.Zero)
                Polling = polling;

            IsAlive = true;
            gcThread = new Thread(new ThreadStart(doLoop));
            gcThread.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            IsAlive = false;
            if (gcThread != null && gcThread.IsAlive)
                gcThread.Join();
        }

        /// <summary>
        /// Does the loop.
        /// </summary>
        private void doLoop()
        {
            while (IsAlive)
            {
                OnLoop();

                Thread.Sleep(Polling);
            }
        }

        #endregion Methods
    }
}