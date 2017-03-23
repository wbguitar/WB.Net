// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2012-05-15 19:12:23 +0200 (mar, 15 mag 2012) $
//Versione: $Rev: 88 $
// ------------------------------------------------------------------------

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.Protocol
{
    /// <summary>
    /// Implementa il parser di un protocollo
    /// </summary>
    public class StreamParser : IProtocol
    {
        #region SyncArgs
        /// <summary>
        /// Contiene le informazioni necessarie alla sincronizzazione sincrona
        /// </summary>
        protected class SyncArgs
        {
            AutoResetEvent waitHandle;
            List<IMessage> retMessage = new List<IMessage>();
            List<MessageValidationException> exception = new List<MessageValidationException>();
            int responseCount;
            int responseCounter = 0;

            /// <summary>
            /// Costruttore
            /// </summary>
            /// <param name="_waitHandle"></param>
            /// <param name="_responseCount"></param>
            public SyncArgs(AutoResetEvent _waitHandle, int _responseCount)
            {
                waitHandle = _waitHandle;
                responseCount = _responseCount;
            }
            /// <summary>
            /// Ritorna il wait handle della richiesta sincrona
            /// </summary>
            public AutoResetEvent WaitHandle
            {
                get { return waitHandle; }
            }
            /// <summary>
            /// Ritorna o imposta il messaggio di risposta
            /// </summary>
            public List<IMessage> RetMessage
            {
                get { return retMessage; }
            }
            /// <summary>
            /// Ritorna o imposta l'eccezione di validazione
            /// </summary>
            public List<MessageValidationException> ValidationException
            {
                get { return exception; }
            }
            /// <summary>
            /// Ritorna il numero di risposte aspettate
            /// </summary>
            public int ResponseCount
            {
                get { return responseCount; }                
            }
            /// <summary>
            /// Ritorna o imposta il numero di risposte ricevute
            /// </summary>
            public int ResponseCounter
            {
                get { return responseCounter; }
                set { responseCounter = value; }
            }
        }
        #endregion

        #region ASyncArgs
        /// <summary>
        /// Contiene le informazioni necessarie alla sincronizzazione asincrona
        /// </summary>
        protected class ASyncArgs
        {


            dAsyncCallback callback;
            object extra;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="_callback"></param>
            /// <param name="_extra"></param>
            public ASyncArgs(dAsyncCallback _callback,object _extra)
            {
                callback = _callback;
                extra = _extra;
            }
            /// <summary>
            /// Ritorna il dato custom
            /// </summary>
            public object Extra
            {
                get { return extra; }
            }
            /// <summary>
            /// Ritorna la callback di risposta
            /// </summary>
            public dAsyncCallback Callback
            {
                get { return callback; }
            }
        }
        #endregion

        #region Private Fields
        private byte[] leaningData = null;
        private bool leaning = false;
        object syncDataReceived;

        IMessageParser parser;
        IStream stream;

        Dictionary<object, SyncArgs> syncMap = new Dictionary<object,SyncArgs>();
        Dictionary<object, ASyncArgs> asyncMap = new Dictionary<object,ASyncArgs>();

        TimeSpan syncTimeout;
        #endregion

        #region Delegates Events
        /// <summary>
        /// Delega la notifica di un evento di errore
        /// </summary>
        /// <param name="ex">Eccezione generata</param>
        public delegate void dOnError(Exception ex);
        /// <summary>
        /// Implementa l'evento OnError
        /// </summary>
        public event dOnError OnError;
        #endregion        
        
        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_stream">IStream di scambio di dati</param>
        /// <param name="_parser">Parser dei messaggi</param>
        public StreamParser(IStream _stream, IMessageParser _parser)
        {
            syncDataReceived = new object();
            syncTimeout = TimeSpan.FromSeconds(5);
            parser = _parser;
            stream = _stream;
            stream.DataReceived += new dDataReceived(stream_DataReceived);
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_stream">IStream di scambio di dati</param>
        /// <param name="_parser">Parser dei messaggi</param>
        /// <param name="_syncTimeout">Timeout di invio sincrono</param>
        public StreamParser(IStream _stream, IMessageParser _parser, TimeSpan _syncTimeout)
        {
            syncDataReceived = new object();
            syncTimeout = _syncTimeout;
            parser = _parser;
            stream = _stream;
            stream.DataReceived += new dDataReceived(stream_DataReceived);
        }
        #endregion 

        #region Private Methods

        private void stream_DataReceived(byte[] data)
        {
            this.DataReceived(data);
        }

        private void SelectCommand(byte[] msgRecv)
        {
            IMessage msg = null;
            MessageValidationException vEx = null;
            try
            {
                //Converto l'array di byte in un messaggio
                msg = parser.ParseMessage(msgRecv);
                //Valido il messaggio
                msg.Validate();

            }
            catch (MessageParseException _pEx)
            {
                //Notifico un errore di Parse del messaggio
                FireOnMessageParseError(_pEx);
            }
            catch (MessageValidationException _vEx)
            {
                vEx = _vEx;
            }

            //Notifico la ricezione di un messaggio
            if (msg != null)
            {
                FireOnMessageReceived(msg, vEx);
            }
        }

        private void FireOnMessageParseError(MessageParseException error)
        {
            if (this.OnMessageParseError  != null)
            {
                this.OnMessageParseError(error);
            }
        }

        private void FireOnMessageReceived(IMessage msg, MessageValidationException vEx)
        {
            if (msg.MessageType == MessageTypes.Response)
            {
                lock (this)
                {
                    object reference = msg.SyncRef;

                    if (syncMap.ContainsKey(reference))
                    {
                        SyncArgs args = syncMap[reference];

                        args.RetMessage.Add(msg);
                        args.ValidationException.Add(vEx);
                        args.ResponseCounter++;
                        if (args.ResponseCounter >= args.ResponseCount)
                        {
                            args.WaitHandle.Set();
                        }
                        return;
                    }

                    if (asyncMap.ContainsKey(reference))
                    {
                        ASyncArgs args = asyncMap[reference];

                        asyncMap.Remove(reference);

                        if (args.Callback != null)
                        {
                            args.Callback(msg, vEx, args.Extra);
                        }
                        return;
                    }
                }

            }

            if (this.MessageReceived != null)
            {
                this.MessageReceived(msg,vEx);
            }
        }

        private void FireOnError(Exception error)
        {
            if (this.OnError != null)
            {
                this.OnError(error);
            }
        }

        private void DataReceived(byte[] byteRecv)
        {
           lock (syncDataReceived)
            {
                try
                {

                    byte[] message;
                    int dataLength = 0;
                    bool repeat = false;
                    //  Uso una struttura ausiliaria per lavorare sui dati:
                    byte[] data;
                    if (leaning)
                    {// Ho dati pendenti:
                        // Concateno i dati pendenti con quelli attualmente ricevuti:
                        data = new byte[leaningData.Length + byteRecv.Length];
                        Array.Copy(leaningData, 0, data, 0, leaningData.Length);
                        Array.Copy(byteRecv, 0, data, leaningData.Length, byteRecv.Length);
                        leaning = false;
                    }
                    else
                    {// Non ho dati pendenti:
                        data = byteRecv;
                    }
                    do
                    {
                        if (!parser.CanReadLength(data))
                        {
                            // Ho meno dati di quelli necessari a ricavare la lunghezza del messaggio:
                            // Aggiorno l'array di dati pendenti:
                            leaningData = new byte[data.Length];
                            Array.Copy(data, 0, leaningData, 0, leaningData.Length);
                            // Notifico che ci sono dati pendenti:
                            leaning = true;
                            // Non devo ripetere:
                            repeat = false;
                        }
                        else
                        {
                            // Estraggo la dimensione del messaggio dai dati:
                            dataLength = parser.GetLength(data);
                            // Controllo quanti dati ho:
                            if ((data.Length) == dataLength)
                            {// Ho ricevuto tutti i dati dichiarati:
                                // Non devo ripetere
                                repeat = false;
                                try
                                {
                                    // Decido quale comando eseguire:
                                    this.SelectCommand(data);
                                }
                                catch (Exception ex)
                                {
                                    FireOnError(ex);
                                }
                            }
                            if ((data.Length) < dataLength)
                            {// Ho meno dati di quelli dichiarati:
                                // Aggiorno l'array di dati pendenti:
                                leaningData = new byte[data.Length];
                                Array.Copy(data, 0, leaningData, 0, leaningData.Length);
                                // Notifico che ci sono dati pendenti:
                                leaning = true;
                                // Non devo ripetere:
                                repeat = false;
                            }
                            if ((data.Length) > dataLength)
                            {// Ho più dati di quelli dichiarati:
                                // Estraggo il messaggio dai dati ricevuti:
                                message = new byte[dataLength];
                                Array.Copy(data, 0, message, 0, dataLength);
                                // Devo ripetere la procedura:
                                repeat = true;
                                try
                                {
                                    // Decido quale comando eseguire:
                                    this.SelectCommand(message);
                                }
                                catch (Exception ex)
                                {
                                    FireOnError(ex);
                                }

                                // Preparo "data" per lavorare sul resto dei dati ricevuti:
                                byte[] remainder = new byte[data.Length - message.Length];
                                Array.Copy(data, message.Length, remainder, 0, remainder.Length);
                                data = remainder;
                                
                            }
                        }
                    } while (repeat);
                }
                catch (Exception ex)
                {
                    FireOnError(ex);
                }
            }

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Pulisce il buffer di ricezione sullo StreamParser
        /// </summary>
        public void ResetInputData()
        {
            lock (syncDataReceived)
            {
                leaning = false;
                leaningData = null;
            }
        }
        #endregion

        #region IProtocol Members

        /// <summary>
        /// Implementa l'evento di ricezione di un messaggio
        /// </summary>
        public event dMessageReceived MessageReceived;
        /// <summary>
        /// Delega la ricezione di un messaggio
        /// </summary>
        public event dOnMessageParseError OnMessageParseError;
        /// <summary>
        /// Invia un messaggio
        /// </summary>
        /// <param name="msg">messaggio da inviare</param>
        public void Send(IMessage msg)
        {
            if(this.parser.SerializeIsSupported())
                stream.Send(this.parser.SerializeMessage(msg));
            else
            stream.Send(msg.GetByteArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="msgOut"></param>
        /// <param name="vEx"></param>
        /// <returns></returns>
        public SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx,TimeSpan timeout)
        {
            
            SyncResult result = SyncResult.NotAvaiable;

            msgOut = null;
            vEx = null;
            if (msgIn.MessageType == MessageTypes.Request)
            {
                object reference = msgIn.SyncRef;
                SyncArgs args = new SyncArgs(new AutoResetEvent(false), msgIn.ResponseCount);

                lock (this)
                {
                    if (syncMap.ContainsKey(reference)) { syncMap.Remove(reference); }
                    if (asyncMap.ContainsKey(reference)) { asyncMap.Remove(reference); }

                    syncMap.Add(reference, args);
                }

                if (this.parser.SerializeIsSupported())
                    stream.Send(this.parser.SerializeMessage(msgIn));
                else
                    stream.Send(msgIn.GetByteArray());
            

                if (args.WaitHandle.WaitOne(timeout))
                {
                    msgOut = args.RetMessage;
                    vEx = args.ValidationException;
                    result = SyncResult.Ok;
                }
                else { result = SyncResult.Timeout; }

                lock (this)
                {
                    if (syncMap.ContainsKey(reference)) { syncMap.Remove(reference); }

                    args.WaitHandle.Close();
                }
            }
            
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="msgOut"></param>
        /// <param name="vEx"></param>
        /// <returns></returns>
        public SyncResult SendSync(IMessage msgIn, out List<IMessage> msgOut, out List<MessageValidationException> vEx)
        {
            return SendSync(msgIn, out msgOut, out vEx, syncTimeout);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="callback"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public ASyncResult SendAsync(IMessage msg, dAsyncCallback callback,object extra)
        {
            if (msg.MessageType == MessageTypes.Request)
            {
                lock (this)
                {
                    object reference = msg.SyncRef;
                    if (syncMap.ContainsKey(reference)) { syncMap.Remove(reference); }
                    if (asyncMap.ContainsKey(reference)) { asyncMap.Remove(reference); }

                    ASyncArgs args = new ASyncArgs(callback, extra);

                    asyncMap.Add(reference, args);

                }
                if (this.parser.SerializeIsSupported())
                    stream.Send(this.parser.SerializeMessage(msg));
                else
                    stream.Send(msg.GetByteArray());

                return ASyncResult.Ok;
            }
            return ASyncResult.NotAvaiable;
        }

        #endregion


    }
}
