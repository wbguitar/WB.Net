// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:20
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Net.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using  WB.Commons.Helpers;
    using  WB.Commons.Serialization;

    using WB.IIIParty.Commons.Net.Sockets;
    using WB.IIIParty.Commons.Protocol;
    using WB.IIIParty.Commons.Protocol.Serialization;

    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        #region Fields

        /// <summary>
        /// The cli
        /// </summary>
        protected TcpClient cli = null;

        /// <summary>
        /// The parser
        /// </summary>
        protected StreamParser parser;

        /// <summary>
        /// The serializer
        /// </summary>
        protected XmlMessageSerializerEx serializer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="parms">The parms.</param>
        /// <exception cref="System.ArgumentNullException">factory</exception>
        public BaseClient(BaseClientFactory factory, params object[] parms)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            RegisterTypes();

            cli = factory.CreateInstance(parms);

            cli.OnConnect += ()=>OnConnect();
            cli.OnDisconnect += ()=>OnDisconnect();
            cli.OnTrace += (s, dir, data, desc)=>OnTrace(s, dir, data, desc);
            cli.ConnectFailure += ()=>OnConnectFailure();

            serializer = new XmlMessageSerializerEx(SerializerInfoExBase.Instance, "MessageId");
            parser = new StreamParser(cli, serializer, TimeSpan.FromSeconds(5));

            parser.MessageReceived += (msg, err)=>MessageReceived(msg, err);
            parser.MessageReceived += (msg, err)=>
                                          {
                                              if (Trace)
                                                  TraceInput(msg, err);
                                          };
            parser.OnError += (err)=>OnError(err);
            parser.OnMessageParseError += (err)=>OnMessageParseError(err);

            Trace = false;
            CliInstanceCreated(cli);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Ritorna lo stato di connessione del socket.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get { return cli != null && cli.IsConnected; }
        }

        /// <summary>
        /// Ritorna l'end point locale del socket (null se socket non connesso)
        /// </summary>
        /// <value>The local end point.</value>
        public EndPoint LocalEndPoint
        {
            get { return cli != null ? cli.LocalEndPoint : null; }
        }

        /// <summary>
        /// Ritorna l'end point remoto del socket (null se socket non connesso)
        /// </summary>
        /// <value>The remote end point.</value>
        public EndPoint RemoteEndPoint
        {
            get { return cli != null ? cli.RemoteEndPoint : null; }
        }

        /// <summary>
        /// Activates <see cref="TraceOutput" /> and <see cref="TraceInput" /> events
        /// </summary>
        /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
        public bool Trace
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Chiude la connessione ma non disabilita il Driver
        /// </summary>
        public void CloseConnection()
        {
            if (cli != null)
                cli.CloseConnection();
        }

        /// <summary>
        /// Abilita il Client TcpIp
        /// </summary>
        /// <param name="connectdelay">Imposta un ritardo sul primo tentativo di connessione</param>
        public void Connect(TimeSpan connectdelay)
        {
            if (cli != null)
                cli.Connect(connectdelay);
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            Connect(TimeSpan.Zero);
        }

        /// <summary>
        /// Abilita il Client TcpIp e attende la prima connessione
        /// </summary>
        /// <param name="connectTimeout">Imposta il timeout di attesa alla prima connessione</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool ConnectWait(TimeSpan connectTimeout)
        {
            return cli != null && cli.ConnectWait(connectTimeout);
        }

        /// <summary>
        /// Disabilita il Client TcpIp
        /// </summary>
        public void Disconnect()
        {
            if (cli != null)
                cli.Disconnect();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            cli.Dispose();
        }

        /// <summary>
        /// Sends the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void Send(IMessage msg)
        {
            if (Trace)
                TraceOutput(msg);
            System.Diagnostics.Debug.Print(msg.SerializeToXml());
            parser.Send(msg);
        }

        /// <summary>
        /// Invia un array di bytes sul Socket
        /// </summary>
        /// <param name="byteToSend">Dati da inviare</param>
        public void Send(byte[] byteToSend)
        {
            if (cli != null)
                cli.Send(byteToSend);
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="extra">The extra.</param>
        /// <returns>ASyncResult.</returns>
        public ASyncResult SendAsync(IMessage msg, dAsyncCallback callback, object extra)
        {
            System.Diagnostics.Debug.Print(msg.SerializeToXml());
            return parser.SendAsync(msg, callback, extra);
        }

        /// <summary>
        /// Sends the sync.
        /// </summary>
        /// <param name="msgIn">The MSG in.</param>
        /// <param name="msgOut">The MSG out.</param>
        /// <param name="vEx">The v ex.</param>
        /// <returns>SyncResult.</returns>
        public SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx)
        {
            System.Diagnostics.Debug.Print(msgIn.SerializeToXml());
            return parser.SendSync(msgIn, out msgOut, out vEx);
        }

        /// <summary>
        /// Attende lo stato di connessione del socket
        /// </summary>
        /// <param name="connectTimeout">Imposta il timeout di attesa dello stato di connesso</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool WaitForConnected(TimeSpan connectTimeout)
        {
            return cli != null && cli.WaitForConnected(connectTimeout);
        }

        /// <summary>
        /// Registers the type of <see cref="IMessage" /> passed as argument
        /// </summary>
        /// <param name="t">the <see cref="IMessage" /> to be registered</param>
        /// <remarks>Raises <see cref="ArgumentException" /> if the type passed is not an IMessage</remarks>
        protected void RegisterType(Type t)
        {
            SerializerInfo.RegisterType(t, typeof (IMessage));
        }

        /// <summary>
        /// When implemented should just call <see cref="RegisterType" />
        /// for each message implemented in the specific protocol
        /// </summary>
        protected abstract void RegisterTypes();

        #endregion Methods

        #region Events
        /// <summary>
        /// Occurs when [cli instance created].
        /// </summary>
        public event Action<TcpClient> CliInstanceCreated = (cli) => { };

        /// <summary>
        /// Event fired when receiving a message
        /// </summary>
        public event Action<IMessage> TraceOutput = (msg) => { };

        /// <summary>
        /// Event fired when sending a message
        /// </summary>
        public event Action<IMessage, MessageValidationException> TraceInput = (msg, err) => { };

        /// <summary>
        /// Occurs when [on connect].
        /// </summary>
        public event TcpClient.ConnectEvent OnConnect = () => { };
        /// <summary>
        /// Occurs when [on disconnect].
        /// </summary>
        public event TcpClient.DisconnectEvent OnDisconnect = () => { };
        /// <summary>
        /// Occurs when [on trace].
        /// </summary>
        public event dOnTrace OnTrace = (s, dir, data, desc) => { };
        /// <summary>
        /// Occurs when [on connect failure].
        /// </summary>
        public event TcpClient.DisconnectEvent OnConnectFailure = () => { };

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event dMessageReceived MessageReceived = (msg, err) => { };
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event StreamParser.dOnError OnError = (ex) => { };
        /// <summary>
        /// Occurs when [on message parse error].
        /// </summary>
        public event dOnMessageParseError OnMessageParseError = (err) => { };
        
        

        #endregion Other
    }
}