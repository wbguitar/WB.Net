// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-10-10 16:39:10 +0200 (gio, 10 ott 2013) $
//Versione: $Rev: 189 $
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WB.IIIParty.Commons.Net.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public class TCPEventArgs : EventArgs
    {

        #region Variable Declaration
        private string mintSocket = "";
        private EndPoint m_EndPoint;
        private byte[] mData;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="socketkey"></param>
        /// <param name="endpoint"></param>
        public TCPEventArgs(byte[] data, string socketkey, EndPoint endpoint)
        {
            mData = data;
            mintSocket = socketkey;
            m_EndPoint = endpoint;
        }
        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data
        {
            get
            {
                return mData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SocketKey
        {
            get
            {
                return mintSocket;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Net.EndPoint EndPoint
        {
            get
            {
                return m_EndPoint;
            }
        }

        #endregion
    }

	/// <summary>Implementa un Server TCP</summary>
	public class TcpServer {

		#region Variable Declaration
		private int mintPort = 0;
		private IPAddress mIPAddress = IPAddress.Any;
		private int mintConnections = 0;
        private int mintBuffer = 16384;
        private Socket listener;
        private Hashtable clientsList = new Hashtable();
        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="port">Porta di ricezione</param>
        public TcpServer(int port)
        {
			mintPort = port;
		}

        /// <summary>Costruttore</summary>
        public TcpServer() { }
        #endregion

        #region Properties

        /// <summary>
        /// Porta di ricezione TCP
        /// </summary>
		public int LocalPort {
			get {return mintPort;}
			set {mintPort = value;}
		}

        /// <summary>
        /// Dimensione del buffer di ricezione
        /// </summary>
        public int InputBuffer
        {
            get { return mintBuffer; }
            set { mintBuffer = value; }
        }

        /// <summary>
        /// Connessioni Correnti
        /// </summary>
        public List<TcpServerConnection> ActiveConnections
        {
            get 
            {
                List<TcpServerConnection> list = new List<TcpServerConnection>();
                foreach(object obj in clientsList.Values)
                {
                    list.Add((TcpServerConnection)obj);
                }
                return list; 
            }
        }

        /// <summary>
        /// Numero di Connessioni Correnti
        /// </summary>
		public int ActiveConnectionsCount {
			get {return mintConnections;}
		}

        /// <summary>
        /// Indirizzo sul quale viene effettuato il Bind
        /// </summary>
		public IPAddress LocalIpAddress {
			get {return mIPAddress;}
			set {mIPAddress = value;}
		}

		#endregion

		#region Events.

		/// <summary>
        /// Delega l'evento BeginConnect
		/// </summary>
		/// <param name="sender">Oggetto TcpServerConnection</param>
		/// <param name="e">Args</param>
		public delegate void ConnectEventHandler(object sender, TCPEventArgs e);

        /// <summary>
        /// Implementa l'evento di connessione
        /// </summary>
		public event ConnectEventHandler BeginConnect;

        /// <summary>
        /// Delega l'evento BeginConnect
        /// </summary>
        /// <param name="sender">Oggetto TcpServerConnection</param>
        /// <param name="e">Args</param>
		public delegate void DisconnectEventHandler(object sender, TCPEventArgs e);
        /// <summary>
        /// Implementa l'evento di disconnessione
        /// </summary>
		public event DisconnectEventHandler BeginDisconnect;

		#endregion

        #region Public Method
        /// <summary>Abilita il Server</summary>
		public void Start() {
			
			if (mintPort < 1) throw new Exception("Invalid port.");

			try 
            {
				// Initialize socket objects.
				listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				listener.Bind(new IPEndPoint(mIPAddress, mintPort));
				listener.Listen(5);

				listener.BeginAccept(new AsyncCallback(BeginAccept), listener);
			} catch (Exception ex) {
				throw ex;
			}
		}

		/// <summary>Disabilita il Server</summary>
		public void Stop() {
			try {
				if (listener != null) {
					Disconnect();  // Diconnette tutti i Clients
					listener.Close();
				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		/// <summary>Disconnette un Client</summary>
        /// <param name="connectionId">Identificativo della connessione</param>
		private void Disconnect(string connectionId) {
			try {
                if ((clientsList[connectionId] != null) && (((TcpServerConnection)clientsList[connectionId]).Connected))
                {
                    ((TcpServerConnection)clientsList[connectionId]).DisconnectSocket(false);
				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		/// <summary>Disconnette tutti i Clients</summary>
		public void Disconnect() {
            Hashtable copy =  (Hashtable)clientsList.Clone();
            foreach (string key in copy.Keys) { Disconnect(key); }
        }

        #endregion

        #region Private Method

        /// <summary>Disconnette un Client da un oggetto Socket</summary>
		/// <param name="socket">Socket</param>
		private void disconnect_socket(Socket socket) {
			try {
				if (!socket.Connected) return;
				socket.Shutdown(SocketShutdown.Both);
				System.Threading.Thread.Sleep(10);
				socket.Close();
			} catch (Exception ex) {
				throw ex;
			}
		}

		

		/// <summary>Evento Connessione Avvenuta</summary>
		private void BeginAccept(IAsyncResult ar) {
			Socket listener = (Socket)ar.AsyncState;
            
			if ((listener == null) || (listener.Handle.ToInt32() == -1)) return;
            
            try {

                Socket client = listener.EndAccept(ar);

				// search for empty client node.
                string key = client.RemoteEndPoint.ToString();
                if (!clientsList.ContainsKey(key))
                {
                    mintConnections++;

                    clientsList.Add(key, new TcpServerConnection(client, this.InputBuffer));                    
                    ((TcpServerConnection)clientsList[key]).OnDisconnect += new TcpServerConnection.DisconnectEventHandler(DisconnectSocket);
                    if (BeginConnect != null) BeginConnect(((TcpServerConnection)clientsList[key]), new TCPEventArgs(null, key, ((TcpServerConnection)clientsList[key]).RemoteEndPoint));

                    ((TcpServerConnection)clientsList[key]).BeginReceive();
						
				}                

				listener.BeginAccept(new AsyncCallback(BeginAccept), listener);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
		}
		

        /// <summary>
        /// Client Disconnesso
        /// </summary>
        /// <param name="sender">Client</param>
        /// <param name="e">Parametri</param>
        /// <param name="DisconnectByServer">Indica se il socket è stato disconnesso dall'implementatore</param>
        private void DisconnectSocket(object sender, TCPEventArgs e, bool DisconnectByServer)
        {
            clientsList.Remove(e.SocketKey);
			--mintConnections;

            if (!DisconnectByServer)
            {
                if (BeginDisconnect != null) BeginDisconnect(sender, e);
            }
        }
        #endregion
    }
}