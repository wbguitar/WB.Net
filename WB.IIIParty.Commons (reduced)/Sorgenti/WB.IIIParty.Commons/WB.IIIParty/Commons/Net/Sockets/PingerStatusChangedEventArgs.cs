using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Net.Sockets
{
    /// <summary>
    /// Contiene i dati da inviare nel caso un dispositivo lanci l'evento
    /// </summary>
    class PingerStatusChangedEventArgs
    {
        private System.Net.IPAddress address = null;
        private bool status = false;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_address">Indirizzo IP del dispositivo che genera l'evento</param>
        /// <param name="_status">Stato del dispositivo (true = connesso, false = disconnesso)</param>
        public PingerStatusChangedEventArgs(System.Net.IPAddress _address, bool _status)
        {
            this.address = _address;
            this.status = _status;
        }

        /// <summary>
        /// Ritorna l'indirizzo del dispositivo
        /// </summary>
        public System.Net.IPAddress Address
        {
            get { return address; }
            private set { address = value; }
        }

        /// <summary>
        /// Ritorna lo stato del dispositivo (true = connesso, false = disconnesso)
        /// </summary>
        public bool Status
        {
            get { return status; }
            private set { status = value; }
        }
    }
}
