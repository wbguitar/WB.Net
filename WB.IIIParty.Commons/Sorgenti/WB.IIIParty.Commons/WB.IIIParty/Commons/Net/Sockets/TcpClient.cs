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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WB.IIIParty.Commons.Protocol;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WB.IIIParty.Commons.Net.Sockets
{
    /// <summary>
    /// Implemenat un Client TCP/IP
    /// </summary>
    public class TcpClient: IStream
    {

        #region Delegati

        /// <summary>
        /// Delega l'evento di connessione del Socket
        /// </summary>
        public delegate void ConnectEvent();
        /// <summary>
        /// Delega l'evento di disconnessione del Socket
        /// </summary>
        public delegate void DisconnectEvent();

        #endregion

        #region Campi privati

        private ManualResetEvent firstConnectEvent;
        /// <summary>
        /// TRUE se il client prevede il tentativo di riconnessione automatica 
        /// in caso di disconnessione. False, altrimenti.
        /// </summary>
        private bool autoreconnect;

        //  Socket di comunicazione.
        private Socket m_clientSocket;
    //  EndPoint
        private IPEndPoint server_ipEndPoint;
    //  Flag di stato del thread di controllo della connessione.  
        private bool m_running;

    //  Flag di controllo della connessione.
        private bool _checkConnection;
        private TimeSpan _reconnectscantime = TimeSpan.FromSeconds(10);
        private TimeSpan _connectdelay = TimeSpan.FromSeconds(0);
    //  Thread di controllo della connessione.
        private System.Threading.Thread _checkConnectionThread;
    //  AsyncCallback provides a way for client applications to complete an asynchronous operation. 
    //  This callback delegate is supplied to the client when the asynchronous operation is initiated. 
    //  The event handler referenced by AsyncCallback contains program logic to finish processing the 
    //  asynchronous task for the client.
    //  AsyncCallback uses the IAsyncResult interface to obtain the status of the asynchronous operation.
    //  The IAsyncResult interface is implemented by classes containing methods that can operate asynchronously. 
    //  It is the return type of methods that initiate an asynchronous operation, such as FileStream.BeginRead, 
    //  and is the type of the third parameter of methods that conclude an asynchronous operation, such as 
    //  FileStream.EndRead. IAsyncResult objects are also passed to methods invoked by AsyncCallback delegates 
    //  when an asynchronous operation completes.
    //  An object that supports the IAsyncResult interface stores state information for an asynchronous operation, 
    //  and provides a synchronization object to allow threads to be signaled when the operation completes.
    //  For a detailed description of how the IAsyncResult interface is used, see the Calling Synchronous Methods 
    //  Asynchronously topic.
    //  Vedi Calling Synchronous Methods Asynchronously. 
        private AsyncCallback m_pfnCallBack;
    //  Oggetto necessario per la ricezione asincrona dei pacchetti.
        SocketStateObject theSocPkt;
        #endregion

        #region Eventi

        /// <summary>
        /// Implementa l'evento di avvenuta ricezione dati
        /// </summary>
        public event dDataReceived DataReceived;

        /// <summary>
        /// Implementa l'evento di avvenuta connessione
        /// </summary>
        public event ConnectEvent OnConnect;

        /// <summary>
        /// Implementa l'evento di avvenuta disconnessione
        /// </summary>
        public event DisconnectEvent OnDisconnect;
        /// <summary>
        /// Implementa l'evento di connessione fallita
        /// </summary>
        public event DisconnectEvent ConnectFailure;
        #endregion 

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="address">Indirizzo Ip del server</param>
        /// <param name="port">Porta di ascolto del server</param>
        /// <param name="reconnectscantime">Tempo di riconnessione automatica</param>
        /// <param name="_autoreconnect"> True per abilitare il tentativo di riconnessione al server.</param>
        /// <param name="inputBufferLength"> Lunghezza in bytes del buffer di input.</param>
        public TcpClient(string address, int port, TimeSpan reconnectscantime, bool _autoreconnect,int inputBufferLength)
        {
            firstConnectEvent = new ManualResetEvent(false);

            _reconnectscantime = reconnectscantime;
            autoreconnect = _autoreconnect;
            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(address);
            }
            catch(Exception e)
            {
                throw e;
                // LOG
            }
        //  Create the end point.

        //  Crea un oggetto di tipo IPEndPoint.
            server_ipEndPoint = new IPEndPoint(ip, port);
        //  Crea un oggetto di tipo Socket.
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

        //  Inzializza tutti gli altri campi della classe.
            m_pfnCallBack = new AsyncCallback(OnDataReceived);
            theSocPkt = new SocketStateObject(this.m_clientSocket, inputBufferLength);
            
            _checkConnection = false;

        }

        /// <summary>
        /// Costruttore AutoReconnect custom -  - Buffer di input a 1024
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="reconnectscantime"></param>
        /// <param name="_autoreconnect"></param>
        public TcpClient(string address, int port, TimeSpan reconnectscantime, bool _autoreconnect):
            this(address, port, reconnectscantime, _autoreconnect, 16384)
        {

        }

        /// <summary>
        /// Costruttore AutoReconnect abilitato - Buffer di input a 1024
        /// </summary>
        /// <param name="address">Indirizzo Ip del server</param>
        /// <param name="port">Porta di ascolto del server</param>
        /// <param name="reconnectscantime">Tempo di riconnessione automatica</param>
        public TcpClient(string address, int port, TimeSpan reconnectscantime):
            this(address,port,reconnectscantime,true)
        {
            
        }

        #endregion

        #region Metodi Privati

        private void FireOnTrace(TraceDirections direction, byte[] data,string description)
        {
            if (OnTrace != null)
            {
                OnTrace(this,direction, data,description);
            }
        }

        private void InternalConnection()
        {
            //  CONDIZIONE:
            //  Se, è necessario eseguire il controllo della connessione e il socket non è connesso,
            if ((_checkConnection) && (!m_clientSocket.Connected))
            {
                //  Allora:
                try
                {
                    //  Esegue la connessione del socket.
                    //							m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //							m_pfnCallBack = new AsyncCallback(OnDataReceived);
                    //							theSocPkt = new SocketStateObject(this.m_clientSocket, 1024);
                    m_clientSocket.Connect(server_ipEndPoint);
                    //  Controlla che la connessione abbia avuto successo.
                    if (m_clientSocket.Connected)
                    {
                        //  Lancia l'evento di OnConnect.
                        FireOnConnect();
                        //  Attesa dei dati.
                        WaitForData();
                    }
                }
                catch (Exception e)
                {
                    if (ConnectFailure != null)
                    {
                        ConnectFailure();
                    }

                    this.ReinizializeSocket();

                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckConnectionThread()
        {
            try
            {
                System.Threading.Thread.Sleep(_connectdelay);

                while (m_running)
                {
                    InternalConnection();
                //  Attesa di 10 secondi.
                    System.Threading.Thread.Sleep(_reconnectscantime);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {

            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void ReinizializeSocket()
        {
            try
            {
                //  Mette a falso il flag per il controllo della connessione; quando il socket è disconesso non serve
                //  controllare la connessione.
                this._checkConnection = false;
                //  Esegue lo shutdown del socket.
                //  Il metodo Shutdown(SocketShutdown.Both) disabilita l'invio e la ricezione sul socket (parametro SocketShutdown.Both). 
                m_clientSocket.Shutdown(SocketShutdown.Both);
                //  Chiude il socket e rilascia le risorse ad esso associate.
                m_clientSocket.Close();
                //  Lancia il metodo che gestisce la disconnessione.
                FireOnDisconnect();
                //  Inizializza il socket e tutti gli oggetti preposti e effettuare la connessione.
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                theSocPkt = new SocketStateObject(this.m_clientSocket, 1024);                
                //  Mette a true il flag per il controllo della connessione.
                this._checkConnection = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this._checkConnection = true;
            }

        }
        #endregion

        #region Metodi Pubblici

        /// <summary>
        /// Ritorna lo stato di connessione del socket.
        /// </summary>
        public bool IsConnected
        {
            get { return this.m_clientSocket.Connected; }
        }

        /// <summary>
        /// Ritorna l'end point locale del socket (null se socket non connesso)
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get { return this.m_clientSocket.LocalEndPoint; }
        }

        /// <summary>
        /// Ritorna l'end point remoto del socket (null se socket non connesso)
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return this.m_clientSocket.RemoteEndPoint; }
        }

        /// <summary>
        /// Chiude la connessione ma non disabilita il Driver
        /// </summary>
        public void CloseConnection()
        {
            //  Chiude il socket e rilascia le risorse ad esso associate.
            m_clientSocket.Disconnect(false);

            //FireOnDisconnect();
        }

        /// <summary>
        /// Abilita il Client TcpIp e attende la prima connessione
        /// </summary>
        /// <param name="connectTimeout">Imposta il timeout di attesa alla prima connessione</param>
        /// <returns></returns>
        public bool ConnectWait(TimeSpan connectTimeout)
        {
            Connect(TimeSpan.FromSeconds(0));

            return WaitForConnected(connectTimeout);
        }

        /// <summary>
        /// Attende lo stato di connessione del socket
        /// </summary>
        /// <param name="connectTimeout">Imposta il timeout di attesa dello stato di connesso</param>
        /// <returns></returns>
        public bool WaitForConnected(TimeSpan connectTimeout)
        {
            return firstConnectEvent.WaitOne(connectTimeout);
        }

        /// <summary>
        /// Abilita il Client TcpIp
        /// </summary>
        /// <param name="connectdelay">Imposta un ritardo sul primo tentativo di connessione</param>
        public void Connect(TimeSpan connectdelay)
        {
            _connectdelay = connectdelay;
            // Imposto a true il flag di Running
            m_running = true;
            //  Mette solo a true il flag di controllo della connessione perchè è il thread di controllo della 
            //  conessione che in base a questa condizione esegue la connessione del socket.
            _checkConnection = true;

            if (autoreconnect)
            {
                //Eseguo il Thread di Controllo
                _checkConnectionThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckConnectionThread));
                _checkConnectionThread.Name = "TCPClient_ConnectionThread_" + this.server_ipEndPoint.ToString();
                _checkConnectionThread.Start();
            }
            else InternalConnection();
        }

        /// <summary>
        /// Disabilita il Client TcpIp
        /// </summary>
        public void Disconnect()
        {
            if (autoreconnect)
            {
                //  Mette a false il flag che specifica lo stato di esecuzione.
                m_running = false;
                //  Esegue l'abort del thread di controllo della connessione.
                if ((_checkConnectionThread != null) && (_checkConnectionThread.IsAlive))
                _checkConnectionThread.Abort();

                //while(this._checkConnectionThread.IsAlive)
                //{
                //    Thread.Sleep(10);
                //}
                //  Mette a falso il flag per il controllo della connessione; quando il socket è disconesso non serve
                //  controllare la connessione.
                this._checkConnection = false;
            }
        //  Esegue lo shutdown del socket.
        //  Il metodo Shutdown(SocketShutdown.Both) disabilita l'invio e la ricezione sul socket (parametro SocketShutdown.Both). 
            if(m_clientSocket.Connected)
            {
                m_clientSocket.Shutdown(SocketShutdown.Both);
            }            
        //  Chiude il socket e rilascia le risorse ad esso associate.
            m_clientSocket.Close();
        //  Lancia il metodo che gestisce la disconnessione.            
            FireOnDisconnect();
            //FireOnDisconnect();
        //  Inizializza il socket e tutti gli oggetti preposti e effettuare la connessione.
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            theSocPkt = new SocketStateObject(this.m_clientSocket, 1024);
        //  Mette a true il flag per il controllo della connessione.
            this._checkConnection = true;
        }


        /// <summary>
        /// Invia un array di bytes sul Socket
        /// </summary>
        /// <param name="byteToSend">Dati da inviare</param>
        public void Send(byte[] byteToSend)
        {
           int packetNumber = this.SendData(byteToSend);
        }

        /// <summary>
        /// Invia un array di bytes sul Socket
        /// </summary>
        /// <param name="byteToSend">Dati da inviare</param>
        /// <returns>Ritorna il numero di bytes da inviare</returns>
        private int SendData(byte[] byteToSend)
        {
            try
            {

                //  Controlla se il socket è connesso.
                if (m_clientSocket.Connected)
                {
                    //  Invia il pacchetto come un array di byte e ritorna il numero di byte inviati.
                    int dataSend = m_clientSocket.Send(byteToSend);

                    FireOnTrace(TraceDirections.Output, byteToSend,this.server_ipEndPoint.ToString());

                    return dataSend;
                }
                else
                {
                    //  Se il socket è disconnesso ritorna 0.
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //logServer.LogMessage(logServer.ERROR, "Send - " + e.ToString(), "TCPClient");
                return 0;
            }
        }

        private void WaitForData()
        {
            try
            {
                //  Start listening to the data asynchronously.
                //  Metodo Socket.BeginReceive Method (Byte[],Int32,Int32,SocketFlags,AsyncCallback,Object)
                //  public IAsyncResult BeginReceive (
                //                                    byte[] buffer,            : An array of type Byte that is the storage location for the received data.
                //                                    int offset,               : The location in buffer to store the received data.
                //                                    int size,                 : The number of bytes to receive.
                //                                    SocketFlags socketFlags,  : A bitwise combination of the SocketFlags values.
                //                                    AsyncCallback callback,   : An AsyncCallback delegate that references the method to invoke when the operation is complete.
                //                                    Object state              : A user-defined object that contains information about the receive operation. 
                //                                                                This object is passed to the EndReceive delegate when the operation is complete.
                //                                    )
                //  Return Value:  An IAsyncResult that references the asynchronous read. 

                m_clientSocket.BeginReceive(theSocPkt.buffer,        //  Buffer di ricezione dei dati.
                                            0,                       //  Offset 0.
                                            theSocPkt.buffer.Length, //  Dimensione del buffer. 
                                            SocketFlags.None,        //  Nessun flag.
                                            m_pfnCallBack,           //  CallBack.
                                            theSocPkt                //  Oggetto che contiene le informazioni circa l'operazione di ricezione.
                                            );
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
                //logServer.LogMessage(logServer.ERROR, "WaitForData - " + se.ToString(), "TCPClient");
            }
        }

    //  Gestione dei dati ricevuti.
        // The following method is called when each asynchronous operation completes.
        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {

                if (this.m_running)
                {
                    //  Crea l'oggetto che deve contenere le informazioni circa l'operazione di ricezione.
                    SocketStateObject theSSO = (SocketStateObject)asyn.AsyncState;
                    //  Determina il numero di byte ricevuti.
                    int iRx = theSSO.workSocket.EndReceive(asyn);
                    if (iRx > 0)
                    {
                        //  Crea un array dimensionato con il numero di byte ricevuti.
                        byte[] dataReceive = new byte[iRx];
                        //  Copia in dataReceive i dati ricevuti che sono stati passati attraverso l'oggetto preposto.
                        Array.Copy(theSSO.buffer, dataReceive, iRx);
                        //  Notifica dell'evento trace.

                        FireOnTrace(TraceDirections.Input, dataReceive,this.server_ipEndPoint.ToString());
                        //  Notifica dell'evento di avvenuta ricezione.
                        FireOnReceive(/*this,*/ dataReceive);
                        //  Si rimette in attesa per una nuova ricezione.
                        WaitForData();
                    }
                    else
                    {
                        this.ReinizializeSocket();
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                this.ReinizializeSocket();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.ReinizializeSocket();
            }
        }
        #endregion

        #region Gestori eventi
        //  Gestione dell'evento di ricezione dei dati.
        private void FireOnReceive(/*object _sender,*/ byte[] _data)
        {
            //Thread.CurrentThread.Name = "TCPClient_ReceiveThread_" + this.server_ipEndPoint.ToString(); ;
            //  Controlla se è avvenuto il cablaggio da parte di un gestore di eventi.
            //if (OnReceive != null)//SOSTITUITO CON QUELLO SOTTO
            //{
            //    //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
            //    OnReceive(_sender, _data);
            //}

            if (DataReceived != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                DataReceived(_data);
            }
        }

        //  Gestione dell'evento di connessione.
        private void FireOnConnect()
        {
            firstConnectEvent.Set();

            //  Controlla se è avvenuto il cablaggio da parte di un gestore di eventi.
            if (OnConnect != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                Thread tDisc = new Thread(new ThreadStart(OnConnect));
                tDisc.Start();
            }
        }

        //  Gestione dell'evento di disconnessione.
        private void FireOnDisconnect()
        {
            firstConnectEvent.Reset();

            //  Controlla se è avvenuto il cablaggio da parte di un gestore di eventi.
            if (OnDisconnect != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                Thread tDisc = new Thread(new ThreadStart(OnDisconnect));
                tDisc.Start();
                
            }
        }
        #endregion

        #region Classe interna
    //  Questa classe serve a generare l'oggetto che contiene le informazioni circa 
    //  l'operazione asincrona di ricezione.
        internal class SocketStateObject
        {
        //  Client socket.
            public Socket workSocket;
        //  Size of receive buffer.
            public int BufferSize;
        //  Receive buffer.
            public byte[] buffer;

            public SocketStateObject(Socket _s, int _bufSize)
            {
                workSocket = _s; //  Serve per riconoscere il giusto socket quando ci sono più client.
                BufferSize = _bufSize;
                buffer = new byte[BufferSize]; 
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disabilita il Client Tcp e libera le risorse allocate
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.m_running = false;

                this.Disconnect();

                this.m_clientSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion 
    
        #region IStream Members

        /// <summary>
        /// Implementa l'evento di Trace
        /// </summary>
        public event dOnTrace OnTrace;

        #endregion
    }
}
