// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:35
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Net.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using  WB.Commons.Helpers;
    using  WB.Commons.Serialization;

    using WB.IIIParty.Commons.Net.Sockets;
    using WB.IIIParty.Commons.Protocol;
    using WB.IIIParty.Commons.Protocol.Serialization;

    /// <summary>
    /// Class BaseRedundantClient
    /// </summary>
    public abstract class BaseRedundantClient : IDisposable
    {
        #region Events

        /// <summary>
        /// Occurs when [cli instance created].
        /// </summary>
        public event Action<RedundantClientTcp> CliInstanceCreated = (cli) => { };

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
        /// Occurs when [message received].
        /// </summary>
        public event dMessageReceived MessageReceived = (msg, err) => { };
        /// <summary>
        /// Occurs when [on message parse error].
        /// </summary>
        public event dOnMessageParseError OnMessageParseError = (err) => { };

        /// <summary>
        /// Occurs when [on server connect].
        /// </summary>
        public event ServerConnectEvent OnServerConnect = (info) => { };
        /// <summary>
        /// Occurs when [on server disconnect].
        /// </summary>
        public event ServerDisconnectEvent OnServerDisconnect = (info) => { };
        #endregion

        #region Fields

        /// <summary>
        /// The cli
        /// </summary>
        protected RedundantClientTcp cli = null;

        /// <summary>
        /// The serializer
        /// </summary>
        protected XmlMessageSerializerEx serializer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Costruttore base per un client ridondato basato su un protocollo xml
        /// </summary>
        /// <param name="factory">factory</param>
        /// <param name="parms">parametri da passare al factory</param>
        /// <exception cref="System.ArgumentNullException">factory</exception>
        /// <remarks>prima del client viene creato il serializer del protocollo ed aggiunto in testa ai parametri passati al client factory
        /// ricordarsi nell'implementazione del factory che il primo e` l'IMessageParser da passare al costruttore del
        /// TcpRedundantClient</remarks>
        public BaseRedundantClient(BaseRedundantClientFactory factory, params object[] parms)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            RegisterTypes();
            serializer = new XmlMessageSerializerEx(SerializerInfoExBase.Instance, "MessageId");

            // aggiungo il serializer ai parametri
            var tempParms = new object[parms.Length + 1];
            tempParms[0] = serializer;
            Enumerable.Range(1, parms.Length).ToList().ForEach(i=>tempParms[i] = parms[i - 1]);
            parms = tempParms;

            cli = factory.CreateInstance(parms);

            cli.OnConnect += ()=>OnConnect();
            cli.OnDisconnect += ()=>OnDisconnect();
            //cli.OnTrace += (s, dir, data, desc)=>OnTrace(s, dir, data, desc);
            //cli.ConnectFailure += ()=>OnConnectFailure();

            cli.MessageReceived += (msg, err)=>MessageReceived(msg, err);
            cli.MessageReceived += (msg, err)=>
                                          {
                                              if (Trace)
                                                  TraceInput(msg, err);
                                          };

            cli.OnMessageParseError += (err)=>OnMessageParseError(err);
            cli.OnConnect += () => OnConnect();
            cli.OnDisconnect += ()=>OnDisconnect();
            cli.OnServerConnect += (info)=>OnServerConnect(info);
            cli.OnServerDisconnect += (info)=>OnServerDisconnect(info);

            Trace = false;

            CliInstanceCreated(cli);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Ritorna le informazioni del server primary
        /// </summary>
        /// <value>The info server primary.</value>
        public ServerInfo? InfoServerPrimary
        {
            get
            {
                if (cli != null)
                    return cli.InfoServerPrimary;

                return null;
            }
        }

        /// <summary>
        /// Ritorna le informazioni del server secondary
        /// </summary>
        /// <value>The info server secondary.</value>
        public ServerInfo? InfoServerSecondary
        {
            get
            {
                if (cli != null)
                    return cli.InfoServerSecondary;

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get
            {
                return cli != null && cli.IsConnected;
            }
        }

        /// <summary>
        /// Ritorna lo stato di connessione al server primario
        /// </summary>
        /// <value><c>true</c> if this instance is primary connected; otherwise, <c>false</c>.</value>
        public bool IsPrimaryConnected
        {
            get { return cli != null && cli.IsConnected; }
        }

        /// <summary>
        /// Ritorna se abilitata la ridondanza di client
        /// </summary>
        /// <value><c>true</c> if this instance is redundant; otherwise, <c>false</c>.</value>
        public bool IsRedundant
        {
            get { return cli != null && cli.IsRedundant; }
        }

        /// <summary>
        /// Ritorna lo stato di connessione al server secondarion
        /// </summary>
        /// <value><c>true</c> if this instance is secondary connected; otherwise, <c>false</c>.</value>
        public bool IsSecondaryConnected
        {
            get { return cli != null && cli.IsSecondaryConnected; }
        }

        /// <summary>
        /// Ritorna l'interfaccia stream del primo client tcp
        /// </summary>
        /// <value>The primary stream.</value>
        public IStream PrimaryStream
        {
            get
            {
                if (cli != null)
                    return cli.PrimaryStream;

                return null;
            }
        }

        /// <summary>
        /// Ritorna l'interfaccia stream del secondo client tcp
        /// </summary>
        /// <value>The secondary stream.</value>
        public IStream SecondaryStream
        {
            get
            {
                if (cli != null)
                    return cli.SecondaryStream;

                return null;
            }
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
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            if (cli != null)
                cli.Connect();
        }

        /// <summary>
        /// Disconnects this instance.
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
        /// Forces the disconnect.
        /// </summary>
        public void ForceDisconnect()
        {
            if (cli != null)
                cli.ForceDisconnect();
        }

        /// <summary>
        /// Sends the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void Send(IMessage msg)
        {
            if (Trace)
                TraceOutput(msg);
            cli.Send(msg);
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="extra">The extra.</param>
        /// <returns>System.Nullable{ASyncResult}.</returns>
        public ASyncResult? SendAsync(IMessage msg, dAsyncCallback callback, object extra)
        {
            if (cli != null)
                return cli.SendAsync(msg, callback, extra);
            return null;
        }

        /// <summary>
        /// Sends the sync.
        /// </summary>
        /// <param name="msgIn">The MSG in.</param>
        /// <param name="msgOut">The MSG out.</param>
        /// <param name="vEx">The v ex.</param>
        /// <returns>System.Nullable{SyncResult}.</returns>
        public SyncResult? SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx)
        {
            if (cli != null)
                return cli.SendSync(msgIn, out msgOut, out vEx);

            msgOut = new List<IMessage>();
            vEx = new List<MessageValidationException>();
            return null;
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
    }
}