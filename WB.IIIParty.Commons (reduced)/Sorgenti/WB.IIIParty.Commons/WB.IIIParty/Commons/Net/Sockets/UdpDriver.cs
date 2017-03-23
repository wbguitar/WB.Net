// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-08-30 16:20:45 +0200 (ven, 30 ago 2013) $
//Versione: $Rev: 177 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WB.IIIParty.Commons.Protocol;

namespace WB.IIIParty.Commons.Net.Sockets
{
    /// <summary>
    /// Implementa un Driver Udp
    /// </summary>
    public class UdpDriver : IStream, IDisposable
    {
        #region Private Field

        private System.Net.Sockets.Socket log_socket;        
        private static ManualResetEvent allDone = new ManualResetEvent(false);       
        Thread readthread;
        private IPEndPoint local;
        private EndPoint localEndPoint;
        private IPEndPoint remote;
        private EndPoint remoteEndPoint;
        bool isRunning;
        private byte[] DATA;
        int inputBufferSize = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localPort">Porta locale di ricezione</param>
        /// <param name="_remotePort">Porta remota di ricezione</param>
        /// <param name="_remoteIpAddress">Indirizzo Ip remoto</param>
        /// <param name="_inputBufferSize">Lunghezza in bytes del buffer di ricezione</param>
        public UdpDriver(int _localPort, int _remotePort, string _remoteIpAddress, int _inputBufferSize)
        {
            this.local = new IPEndPoint(IPAddress.Any, _localPort);
            this.localEndPoint = (EndPoint)local;
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
            inputBufferSize = _inputBufferSize;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localPort">Porta locale di ricezione</param>
        /// <param name="_remotePort">Porta remota di ricezione</param>
        /// <param name="_remoteIpAddress">Indirizzo Ip remoto</param>
        public UdpDriver(int _localPort, int _remotePort, string _remoteIpAddress)
        {
            this.local = new IPEndPoint(IPAddress.Any, _localPort);
            this.localEndPoint = (EndPoint)local;
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localPort">Porta locale di ricezione</param>
        /// <param name="localIpAddress">Indirizzo Ip locale</param>
        /// <param name="_remotePort">Porta remota di ricezione</param>
        /// <param name="_remoteIpAddress">Indirizzo Ip remoto</param>
        public UdpDriver(int _localPort, string localIpAddress, int _remotePort, string _remoteIpAddress)
        {
            this.local = new IPEndPoint(IPAddress.Parse(localIpAddress), _localPort);
            this.localEndPoint = (EndPoint)local;
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
        }

        #endregion

        #region Properties

        /// <summary>
        /// EndPoint locale
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get
            {
                return localEndPoint;
            }
        }

        /// <summary>
        /// EndPoint remoto
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get
            {
                return remoteEndPoint;
            }
        }

        /// <summary>
        /// Indica se il Driver è in esecuzione
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        /// <summary>
        /// Ritorna la lunghezza in bytes del buffer di ricezione
        /// </summary>
        public int InputBufferSize
        {
            get
            {
                return inputBufferSize;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Avvia il driver Udp
        /// </summary>
        public void Start()
        {
            if (!isRunning)
            {
                this.log_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                
                if (inputBufferSize != 0)
                {
                    this.log_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, inputBufferSize);
                }
                this.log_socket.Bind(localEndPoint);

                //start listening thread
                this.readthread = new Thread(new ThreadStart(this.WaitRead));
                this.readthread.Start();

                isRunning = true;
            }
        }

        /// <summary>
        /// ferma il driver Udp
        /// </summary>
        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;

                this.readthread.Abort();
                this.readthread.Join();
                this.log_socket.Shutdown(SocketShutdown.Both);
                this.log_socket.Close();
                
            }
        }

        /// <summary>
        /// Modifica la porta di ricezione locale
        /// </summary>
        /// <param name="_localaddress"></param>
        /// <param name="_localPort"></param>
        public void ChangeLocalAddress(string _localaddress,int _localPort)
        {
            bool lastisrunning = this.isRunning;
            if (lastisrunning) Stop();

            //restart listening with new port            
            this.local = new IPEndPoint(IPAddress.Parse(_localaddress), _localPort);
            this.localEndPoint = (EndPoint)local;

            if (lastisrunning) Start();

        }

        /// <summary>
        /// Modifica la porta di ricezione locale
        /// </summary>
        /// <param name="_localPort"></param>
        public void ChangeLocalPort(int _localPort)
        {
            bool lastisrunning = this.isRunning;
            if (lastisrunning) Stop();

            //restart listening with new port            
            this.local = new IPEndPoint(IPAddress.Any, _localPort);
            this.localEndPoint = (EndPoint)local;

            if (lastisrunning) Start();
            
        }

        /// <summary>
        /// Modifica l'indirizzo Ip Remoto
        /// </summary>
        /// <param name="_remotePort"></param>
        /// <param name="_remoteIpAddress"></param>
        public void ChangeRemoteAddress(int _remotePort, string _remoteIpAddress)
        {
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
        }

        /// <summary>
        /// Invia un array di bytes sul socket
        /// </summary>
        /// <param name="msg">Array di bytes</param>
        /// <returns>Ritorna il numero di bytes inviati</returns>
        public int SendData(byte[] msg)
        {
            int dataSend = this.log_socket.SendTo(msg, msg.Length,SocketFlags.None, remoteEndPoint);
            
            FireOnTrace(TraceDirections.Output, msg,remoteEndPoint.ToString());

            return dataSend;
        }

        #endregion

        #region Private Methods

        private void FireOnTrace(TraceDirections direction, byte[] data,string description)
        {
            if (OnTrace != null)
            {
                OnTrace(this,direction, data, description);
            }
        }

        private void WaitRead()
        {
            try
            {
                this.DATA = new byte[short.MaxValue];
                //setting socket connection                
                do
                {
                    if (this.log_socket.Available > 0)
                    {
                        //read data from buffer
                        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.None,0);
                        EndPoint endPoint = (EndPoint)ipEndPoint;
                        int dataRcvSize = 0;
                        try
                        {
                            dataRcvSize = this.log_socket.ReceiveFrom(DATA, 0, DATA.Length, SocketFlags.None, ref endPoint);
                        }
                        catch (SocketException ex1)
                        {
                            Console.WriteLine(ex1.Message);
                        }
                        //process data
                        byte[] dataRcv = new byte[dataRcvSize];
                        Array.Copy(DATA, 0, dataRcv, 0, dataRcvSize);
                        FireOnTrace(TraceDirections.Input, dataRcv,endPoint.ToString());
                        FireOnDataReceived(dataRcv);
                        //clear memory data                        
                        for (int i = 0; i < this.DATA.Length; i++)
                        {
                            DATA[i] = 0;
                        }
                    }
                    else
                        Thread.Sleep(1);
                }
                while (isRunning);
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void FireOnDataReceived(byte[] dataRcv)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(dataRcv);
            }
        }

        #endregion

        #region IStream Members

        /// <summary>
        /// Implementa l'evento di ricezione di un array di bytes
        /// </summary>
        public event dDataReceived DataReceived;

        /// <summary>
        /// Invia un array di bytes sul socket
        /// </summary>
        /// <param name="msg">Array di bytes</param>
        /// <returns></returns>
        public void Send(byte[] msg)
        {
            this.SendData(msg);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Ferma il driver e libera le risorse
        /// </summary>
        public void Dispose()
        {
            Stop();
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
