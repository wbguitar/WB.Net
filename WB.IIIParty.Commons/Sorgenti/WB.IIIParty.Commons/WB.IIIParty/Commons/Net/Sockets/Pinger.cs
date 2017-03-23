using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace WB.IIIParty.Commons.Net.Sockets
{
    /// <summary>
    /// Diagnostica lo stato di connessione alla rete dei dispositivi ed avvisa quando cambiano stato
    /// </summary>
    class Pinger : IDisposable
    {
        // PRIVATE ATTRIBUTES

        private IPAddress ipAddress;
        private TimeSpan scanTime;
        private int ttl;
        private int retryCount;
        private bool status = false;
        //private bool secondChance = false;
        private System.Threading.Thread r;
        Ping request = new Ping();

        // EVENTS

        public delegate void EventHandler(object sender, PingerStatusChangedEventArgs argomenti); 
        public event EventHandler StatusChanged;

        // CONSTRUCTORS 

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="ip">Indirizzo IP del dispositivo da controllare</param>
        /// <param name="sTime">Tempo di gap tra un ping e l'altro</param>
        /// <param name="rCount">numero di tentativi falliti prima di dichiarare che il dispositivo è disconnesso</param>
        public Pinger(IPAddress ip, TimeSpan sTime, int rCount)
        {
            this.ipAddress = ip;
            this.scanTime = sTime;
            this.retryCount = rCount;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="ip">Indirizzo IP del dispositivo da controllare</param>
        /// <param name="sTime">Tempo di gap tra un ping e l'altro</param>
        /// <param name="rCount">numero di tentativi falliti prima di dichiarare che il dispositivo è disconnesso</param>
        /// <param name="Ttl">numero massimo di router che possono essere attraversati dal pacchetto</param>
        public Pinger(IPAddress ip, TimeSpan sTime, int rCount, int Ttl)
        {
            this.ipAddress = ip;
            this.scanTime = sTime;
            this.ttl = Ttl;
            PingOptions options = new PingOptions();
            options.Ttl = Ttl;
        }

        // PROPERTIES

        /// <summary>
        /// Ritorna l'indirizzo IP
        /// </summary>
        public IPAddress IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        /// <summary>
        /// Ritorna il tempo di scansione
        /// </summary>
        public TimeSpan ScanTime
        {
            get { return scanTime; }
            set { scanTime = value; }
        }

        /// <summary>
        /// Ritorna il time to live
        /// </summary>
        public int Ttl
        {
            get { return ttl; }
            set { ttl = value; }
        }

        /// <summary>
        /// ritorna lo stato ipotetico del dispositivo
        /// </summary>
 /*       public bool SecondChance
        {
            get { return secondChance; }
        }*/

        /// <summary>
        /// ritorna lo stato misurato del dispositivo
        /// </summary>
        public bool Status
        {
            get { return status; }
        }

        // METHODS

        /// <summary>
        /// Lancia il Thread
        /// </summary>
        public void Start()
        {
            r = new System.Threading.Thread(new System.Threading.ThreadStart(LifeChanged));
            r.Start();
        }

        /// <summary>
        /// Ferma il thread
        /// </summary>
        public void Stop()
        {
            r.Abort();
        }

        /// <summary>
        /// Verifica se l'oggetto si è iscritto all'evento ed in caso positivo lo genera
        /// </summary>
        private void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                PingerStatusChangedEventArgs argomenti = new PingerStatusChangedEventArgs(IpAddress, status);
                this.StatusChanged(this, argomenti); // lancia l'evento
            }
        }

        /// <summary>
        /// Pinga il dispositivo e ne ritorna lo stato
        /// </summary>
        private bool IsAlive()
        {
            PingReply response = request.Send(IpAddress, Ttl);
            if (response.Status == IPStatus.Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verifica lo stato del dispositivo
        /// </summary>
        private void LifeChanged ()
        {
            try
            {
                int failureAttempts = 0;
                while (true)
                {
                    bool actualStatus = IsAlive();
                    if (status != actualStatus)
                    {
                        if (status == false || failureAttempts > retryCount)
                        {
                            status = actualStatus;
                            OnStatusChanged();
                            failureAttempts = 0;
                        }
                        else
                            failureAttempts++;
                    }
                    System.Threading.Thread.Sleep(scanTime);
                }
            }
            catch (System.Threading.ThreadAbortException) { }
        }

        #region IDisposable Members

        public void Dispose()
        {
            request.Dispose();
        }

        #endregion
    }
}