// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-10-17 16:08:42 +0200 (gio, 17 ott 2013) $
//Versione: $Rev: 191 $
// ------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using WB.IIIParty.Commons.Protocol;

namespace WB.IIIParty.Commons.Net.Sockets
{

    /// <summary>
    /// Implementa una connessione TCP/IP lato Server
    /// </summary>
	public class TcpServerConnection: IStream {

		#region Events.

        /// <summary>
        /// Avvenuta ricezione dati: evento da concatenare all'evento OnReceive  
        /// </summary>
        public event dDataReceived DataReceived;

		/// <summary>
		/// Notifica la Disconnessione di un Client
		/// </summary>
		/// <param name="sender">ServerTCP</param>
		/// <param name="e">Parametri</param>
        /// <param name="DisconnectByServer">Indica se l'implementatore ha richiamato la disconnessione del Socket</param>
        public delegate void DisconnectEventHandler(object sender, TCPEventArgs e, bool DisconnectByServer);
        /// <summary>
        /// Delega la ricezione di un evento OnDisconnect
        /// </summary>
		public event DisconnectEventHandler OnDisconnect;

		#endregion

        #region Variable Declaration
        private Socket msockClient;
		private int mintID;
        private int tcpConnectionId;

		private byte[] mbytBuffer; // packet data buffer.
		private byte[] mData = new byte[0]; // request buffer.
        #endregion
        
        #region Proprietà
        /// <summary>
        /// Ritorna l'identificativo univoco della connessione
        /// </summary>
        public int ConnectionId
        {
			get { return mintID; }
		}

        /// <summary>
        /// Ritorna l'identificativo univoco della connessione
        /// </summary>
        public int Handle
        {
            get { return tcpConnectionId; }
        }

        /// <summary>
        /// Client Connesso
        /// </summary>
		public bool Connected {
			get {
				if (msockClient == null) return false;
				return msockClient.Connected;
			}
		}
		
        /// <summary>
        /// Client Remoto
        /// </summary>
		public System.Net.EndPoint RemoteEndPoint {
			get { return msockClient.RemoteEndPoint; }
		}
		#endregion

        #region Constructor
        
        /// <summary>Associa la classe TcpServerConnection a un Soket esistente</summary>
        /// <param name="client">Socket Connesso</param>
        public TcpServerConnection(Socket client,int inputBuffer)
        {
            client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            tcpConnectionId = client.Handle.ToInt32();

            // Inizializzazione
            msockClient = client;
            mbytBuffer = new Byte[inputBuffer];
        }

        #endregion

        #region Public Method

        public void BeginReceive()
        {
            // Inizio a ricevere Dati
            msockClient.BeginReceive(mbytBuffer, 0, mbytBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceive), msockClient);
        }

        /// <summary>Invia dati al Socket</summary>
        /// <param name="byte_data">Dati da inviare</param>
        public void Send(byte[] byte_data)
        {
			try {
				if ((msockClient != null) && (msockClient.Connected)) {
					msockClient.Send(byte_data, byte_data.Length, 0);
                    FireOnTrace(TraceDirections.Output, byte_data,this.RemoteEndPoint.ToString());
				}
			} catch(Exception ex) {
				throw ex;
			}
		}

		/// <summary>Disconnette tutti i Sockets</summary>
		public void DisconnectSocket(bool disconnectByServer) {
			if (msockClient == null) return;

			TCPEventArgs args = new TCPEventArgs(null, this.RemoteEndPoint.ToString(), msockClient.RemoteEndPoint);

			try {
				if (msockClient.Connected) {
					mintID = -1; // Prevents BeginReceive from double disconnecting.
					msockClient.Shutdown(SocketShutdown.Both);
					System.Threading.Thread.Sleep(10);
					msockClient.Close();
					msockClient = null;
				}
			} catch (Exception ex) {
				throw ex;
			}

			// Evento disconnessione
            if (OnDisconnect != null) OnDisconnect(this, args, disconnectByServer);
        }
        #endregion

        #region Private Method

        private void FireOnTrace(TraceDirections direction, byte[] data,string description)
        {
            if (OnTrace != null)
            {
                OnTrace(this,direction, data,description);
            }
        }

        private void BeginReceive(IAsyncResult ar) {
			Socket client = (Socket) ar.AsyncState;

			if (mintID < 0) return;

			// Ci sono dati?
            try
            {
                int nBytesRec = client.EndReceive(ar);
                if (nBytesRec > 0)
                {

                    byte[] dataread = new byte[nBytesRec];
                    Array.Copy(mbytBuffer, dataread, nBytesRec);

                    FireOnTrace(TraceDirections.Input, dataread,this.RemoteEndPoint.ToString());

                    // Invio i dati ricevuti
                   // if (BeginRead != null) BeginRead(this, new TCPEventArgs(dataread, this.RemoteEndPoint.ToString(), client.RemoteEndPoint));
                    if (DataReceived != null) DataReceived(dataread);

                    // Se la connessione è UP mi rimetto in ricezione
                    if (client.Connected) client.BeginReceive(mbytBuffer, 0, mbytBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceive), client);
                }
                else
                {
                    // Se non ci sono dati probabilmente la connessione è stata chiusa
                    DisconnectSocket(false);
                    //TCPEventArgs args = new TCPEventArgs(null, this.RemoteEndPoint.ToString(), msockClient.RemoteEndPoint);
                    //if (BeginDisconnect != null) BeginDisconnect(this, args);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                DisconnectSocket(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IStream Members

        /// <summary>
        /// Implementa l'evento di Trace del flusso di dati
        /// </summary>
        public event dOnTrace OnTrace;

        #endregion
    }
}