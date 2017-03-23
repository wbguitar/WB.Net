// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:23
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.Net.Xml
{
    using System.Collections.Generic;

    using  WB.Commons.Serialization;

    using WB.IIIParty.Commons.Protocol;
    using WB.IIIParty.Commons.Protocol.Serialization;

    /// <summary>
    /// Class ServerConnector
    /// </summary>
    public class ServerConnector
    {
        #region Fields

        /// <summary>
        /// The _MSG proc
        /// </summary>
        private readonly IMessageProcessor _msgProc;

        /// <summary>
        /// The _parser
        /// </summary>
        private readonly StreamParser _parser;

        /// <summary>
        /// The _serializer
        /// </summary>
        private XmlMessageSerializerEx _serializer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnector"/> class.
        /// </summary>
        /// <param name="sp">The sp.</param>
        /// <param name="ser">The ser.</param>
        /// <param name="msgProc">The MSG proc.</param>
        public ServerConnector(StreamParser sp, XmlMessageSerializerEx ser, IMessageProcessor msgProc)
        {
            _parser = sp;
            _serializer = ser;
            _msgProc = msgProc;

            _parser.MessageReceived += (msg, err)=>MessageReceived(msg, err);
            _parser.OnError += (err)=>OnError(err);
            _parser.OnMessageParseError += (err)=>OnMessageParseError(err);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>The message processor.</value>
        public IMessageProcessor MessageProcessor
        {
            get { return _msgProc; }
        }

        #endregion Properties

        //public StreamParser Parser { get { return _parser; } }
        //public XmlMessageSerializer Serializer { get { return _serializer; } }

        #region Events

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event dMessageReceived MessageReceived = (msg, err) => { };
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event StreamParser.dOnError OnError = (err) => { };
        /// <summary>
        /// Occurs when [on message parse error].
        /// </summary>
        public event dOnMessageParseError OnMessageParseError = (err) => { };

        #endregion
        #region Methods

        /// <summary>
        /// Sends the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void Send(IMessage msg)
        {
            _parser.Send(msg);
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
            return _parser.SendAsync(msg, callback, extra);
        }

        /// <summary>
        /// Sends the sync.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="msgOut">The MSG out.</param>
        /// <param name="vEx">The v ex.</param>
        /// <returns>SyncResult.</returns>
        public SyncResult SendSync(IMessage msg, out List<IMessage> msgOut, out List<MessageValidationException> vEx)
        {
            return _parser.SendSync(msg, out msgOut, out vEx);
        }

        #endregion Methods
    }
}