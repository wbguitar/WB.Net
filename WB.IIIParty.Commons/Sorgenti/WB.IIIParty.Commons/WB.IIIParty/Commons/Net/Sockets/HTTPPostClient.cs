// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Papi Rudy
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2012-09-24 09:52:51 +0200 (lun, 24 set 2012) $
//Versione: $Rev: 112 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WB.IIIParty.Commons.Protocol;

using System.Net;
using System.IO;

namespace WB.IIIParty.Commons.Net.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public class HTTPPostClient : IStream
    {

        // The RequestState class passes data across async calls.
        public class RequestState
        {
            public WebRequest Request;
        
            public RequestState()
            {
                Request = null;
            }
        }


        #region Private Field

        private string  proxyIp = "";
        private int     proxyPort = 0;
        private string  userNameForProxy = "";
        private string  passwordForProxy = "";
        private string  dominioForProxy = "";

        private string  uri = @"http://127.0.0.1:80/cgi-bin/tt_rcv";

        private int timeout;

        private WebProxy    webProxy;
        private HttpWebRequest  webRequest;
        private WebResponse webResponse;
        #endregion

        #region Constructors
        /// <summary>
        /// Costruttore connessione con proxy.
        /// </summary>
        /// <param name="_url">Indirizzo server web.</param>
        /// <param name="_timeout">Timeout</param>
        /// <param name="_proxyIp">Indirizzo del proxy.</param>
        /// <param name="_proxyPort">Porta del proxy.</param>
        /// <param name="_userNameForProxy">Credenziali di accesso per il proxy, nome utente</param>
        /// <param name="_passwordForProxy">Credenziali di accesso per il proxy, password</param>
        /// <param name="_dominioForProxy">Credenziali di accesso per il proxy, dominio</param>
        public HTTPPostClient(string _url, int _timeout, string _proxyIp, int _proxyPort, string _userNameForProxy, string _passwordForProxy, string _dominioForProxy)
        {
            this.uri                    = _url;
            this.timeout                = _timeout;
            this.proxyIp                = _proxyIp;
            this.proxyPort              = _proxyPort;
            this.userNameForProxy       = _userNameForProxy;
            this.passwordForProxy       = _passwordForProxy;
            this.dominioForProxy        = _dominioForProxy;

            this.webProxy               = new WebProxy(this.proxyIp, this.proxyPort);
            this.webProxy.Credentials   = new NetworkCredential(this.userNameForProxy, this.passwordForProxy, this.dominioForProxy);
        }
        /// <summary>
        /// Costruttore connessione senza proxy.
        /// </summary>
        /// <param name="_url">Indirizzo server web.</param>
        /// <param name="_timeout">Timeout</param>
        public HTTPPostClient(string _url, int _timeout)
        {
            this.uri = _url;
            this.timeout = _timeout * 1000;
            //this.webRequest = WebRequest.Create(this.uri);
            //this.webRequest.Timeout = timeout;
            //this.webRequest.ContentType = "application/x-www-form-urlencoded";
            //this.webRequest.Method = "POST";
        }

        #endregion

        #region Events
        /// <summary>
        /// Gestione dell'evento di ricezione dei dati.
        /// </summary>
        /// <param name="_data"></param>
        private void OnDataReceived(byte[] _data)
        {
            if (DataReceived != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                DataReceived(_data);
            }
        }

        /// <summary>
        /// Gestione dell'evento di ricezione dei dati.
        /// </summary>
        /// <param name="_data"></param>
        private void OnAsyncDataReceived(byte[] _data)
        {
            if (AsyncDataReceived != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                AsyncDataReceived(_data);
            }
        }
        /// <summary>
        /// Gestione dell'evento di Trace.
        /// </summary>
        /// <param name="_direction"></param>
        /// <param name="_data"></param>
        /// <param name="_deascription"></param>
        private void FireOnTrace(TraceDirections _direction, byte[] _data, string _deascription)
        {
            if (OnTrace != null)
            {
                OnTrace(this, _direction, _data, _deascription);
            }
        }

        #endregion

        #region IStream Members

        /// <summary>
        /// 
        /// </summary>
        public event dOnTrace OnTrace;
        /// <summary>
        /// 
        /// </summary>
        public event dDataReceived DataReceived;
        /// <summary>
        /// 
        /// </summary>
        public event dDataReceived AsyncDataReceived;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(byte[] msg)
        {
            Stream os = null;
            try
            { // send the Post

                // Create the state object.
                RequestState rs = new RequestState();

                this.webRequest = (HttpWebRequest)WebRequest.Create(this.uri);
                this.webRequest.ContentType = "application/x-www-form-urlencoded";
                this.webRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                this.webRequest.Credentials = null;
                this.webRequest.ImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.None;
                this.webRequest.PreAuthenticate = false;
                this.webRequest.ServicePoint.Expect100Continue = false;
                this.webRequest.ServicePoint.UseNagleAlgorithm = false;
                this.webRequest.Method = "POST";
                this.webRequest.ContentLength = msg.Length;   //Count bytes to send
                this.webRequest.Timeout = this.timeout;
                this.webRequest.Proxy = null;
                this.webRequest.UnsafeAuthenticatedConnectionSharing = true;
                this.webRequest.KeepAlive = true;

                rs.Request = webRequest;
                
                int count = 0;
                while (count < msg.Length)
                {
                    os.WriteByte(msg[count++]);
                }

                FireOnTrace(TraceDirections.Output, msg, this.uri);

                // Issue the async request.
                IAsyncResult r = (IAsyncResult)webRequest.BeginGetResponse(
                   new AsyncCallback(RespCallback), rs);

            }
            catch (WebException ex)
            {
                throw ex;
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }
        }

        private void RespCallback(IAsyncResult ar)
        {
            Stream sr = null;
            try
            { // get the response
                WebResponse resp = ((RequestState)ar.AsyncState).Request.EndGetResponse(ar);
                
                if (resp != null)
                {
                    using (resp)
                    {
                        sr = resp.GetResponseStream();
                        sr.WriteTimeout = this.timeout;
                        sr.ReadTimeout = this.timeout;
                        int val = 0;
                        List<byte> msgReceive = new List<byte>();

                        while ((val = sr.ReadByte()) != -1)
                        {
                            if (val != -1)
                            {
                                msgReceive.Add((byte)val);
                            }
                        }

                        byte[] data = msgReceive.ToArray();

                        string a = Encoding.UTF8.GetString(data);

                        OnAsyncDataReceived(data);

                        FireOnTrace(TraceDirections.Input, data, this.uri);
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }                        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Send(byte[] msg)
        {
            Stream os = null;
            try
            { // send the Post

                this.webRequest = (HttpWebRequest)WebRequest.Create(this.uri);
                this.webRequest.ContentType = "application/x-www-form-urlencoded";
                this.webRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                this.webRequest.Credentials = null;
                this.webRequest.ImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.None;
                this.webRequest.PreAuthenticate = false;
                this.webRequest.ServicePoint.Expect100Continue = false;
                this.webRequest.ServicePoint.UseNagleAlgorithm= false;
                this.webRequest.Method = "POST";
                this.webRequest.ContentLength = msg.Length;   //Count bytes to send
                this.webRequest.Timeout = this.timeout;
                this.webRequest.Proxy = null;
                this.webRequest.UnsafeAuthenticatedConnectionSharing = true;
                this.webRequest.KeepAlive = true;
                os = this.webRequest.GetRequestStream();
                os.ReadTimeout = this.timeout;
                os.WriteTimeout = this.timeout;
                //os.Write(msg, 0, msg.Length);         //Send it
                int count = 0;
                while (count < msg.Length)
                {
                    os.WriteByte(msg[count++]);
                }
                FireOnTrace(TraceDirections.Output, msg, this.uri);
                
            }
            catch (WebException ex)
            {
                throw ex;
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            Stream sr = null;
            try
            { // get the response
                this.webResponse = this.webRequest.GetResponse();
                if (this.webResponse != null)
                {
                    using (this.webResponse)
                    {
                        sr = this.webResponse.GetResponseStream();
                        sr.WriteTimeout = this.timeout;
                        sr.ReadTimeout = this.timeout;
                        int val = 0;
                        List<byte> msgReceive = new List<byte>();
                        
                        while ((val = sr.ReadByte()) != -1)
                        {
                            if (val != -1)
                            {
                                msgReceive.Add((byte)val);
                            }
                        }

                        byte[] data = msgReceive.ToArray();

                        string a = Encoding.UTF8.GetString(data);

                        OnDataReceived(data);

                        FireOnTrace(TraceDirections.Input, data, this.uri);
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        #endregion
    }
}