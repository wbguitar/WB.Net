// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:24
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Net.Xml
{
    using System;
    using System.Collections.Generic;

    using  WB.Commons.Helpers;
    using  WB.Commons.Serialization;

    using WB.IIIParty.Commons.Net.Sockets;
    using WB.IIIParty.Commons.Protocol;
    using WB.IIIParty.Commons.Protocol.Serialization;

    /// <summary>
    /// Class BaseServer
    /// </summary>
    /// <typeparam name="TFactory">The type of the T factory.</typeparam>
    public abstract class BaseServer<TFactory> : IMessageProcessor
        where TFactory : BaseServerFactory, new()
    {
        #region Fields

        /// <summary>
        /// The connections
        /// </summary>
        protected Dictionary<TcpServerConnection, ServerConnector> Connections = 
            new Dictionary<TcpServerConnection, ServerConnector>();

        /// <summary>
        /// The SRV
        /// </summary>
        private readonly TcpServer srv;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseServer{TFactory}"/> class.
        /// </summary>
        public BaseServer()
        {
            var factory = new TFactory();

            //if (factory == null)
            //    throw new ArgumentNullException("factory");

            RegisterTypes();

            srv = factory.CreateInstance();

            srv.BeginConnect += srv_BeginConnect;
            srv.BeginDisconnect += srv_BeginDisconnect;

            srv.BeginConnect += (s, e)=>BeginConnect(s, e);
            srv.BeginDisconnect += (s, e)=>BeginDisconnect(s, e);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the connector.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <returns>ServerConnector.</returns>
        public ServerConnector GetConnector(TcpServerConnection conn)
        {
            if (Connections.ContainsKey(conn))
                return Connections[conn];

            return null;
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>IMessage.</returns>
        public abstract IMessage ProcessMessage(IMessage msg);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (srv != null)
                srv.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (srv != null)
                srv.Stop();
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="t">The t.</param>
        protected void RegisterType(Type t)
        {
            SerializerInfo.RegisterType(t, typeof (IMessage));
        }

        /// <summary>
        /// When implemented should just call <see cref="RegisterType" />
        /// passing the types of messages in the specific protocol
        /// </summary>
        protected abstract void RegisterTypes();

        /// <summary>
        /// Handles the BeginConnect event of the srv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TCPEventArgs"/> instance containing the event data.</param>
        private void srv_BeginConnect(object sender, TCPEventArgs e)
        {
            var conn = sender as TcpServerConnection;
            var serializer = new XmlMessageSerializerEx(SerializerInfoExBase.Instance, "MessageId");
            var parser = new StreamParser(conn, serializer);
            var connector = new ServerConnector(parser, serializer, this);
            Connections[conn] = connector;
            conn.OnTrace += OnTrace;

            parser.MessageReceived += (msg, err)=>MessageReceived(msg, err);
            parser.OnError += (err)=>OnError(err);
            parser.OnMessageParseError += (err)=>OnMessageParseError(err);

            //BeginConnect(conn, e);
        }

        /// <summary>
        /// Handles the BeginDisconnect event of the srv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TCPEventArgs"/> instance containing the event data.</param>
        private void srv_BeginDisconnect(object sender, TCPEventArgs e)
        {
            try
            {
                var conn = sender as TcpServerConnection;
                Connections.Remove(conn);

                //BeginDisconnect(sender, e);
            }
            catch (Exception exc)
            {
            }
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Occurs when [begin connect].
        /// </summary>
        public event TcpServer.ConnectEventHandler BeginConnect = (s, e) => { };
        /// <summary>
        /// Occurs when [begin disconnect].
        /// </summary>
        public event TcpServer.DisconnectEventHandler BeginDisconnect = (s, e) => { };

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event dMessageReceived MessageReceived = (msg, err) => { };
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event StreamParser.dOnError OnError = (err) =>
        {

        };
        /// <summary>
        /// Occurs when [on message parse error].
        /// </summary>
        public event dOnMessageParseError OnMessageParseError = (exc) => { };
        /// <summary>
        /// Occurs when [on trace].
        /// </summary>
        public event dOnTrace OnTrace = (str, dir, data, desc) => { };
        

        #endregion Other
    }
}