// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:18
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Loggers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using WB.IIIParty.Commons.Logger;
    using WB.IIIParty.Commons.Protocol;

    using TraceDirections = WB.IIIParty.Commons.Logger.TraceDirections;

    /// <summary>
    /// Class TxtTraceLog
    /// </summary>
    public class TxtTraceLog : ITraceLog, IDisposable
    {
        #region Fields

        /// <summary>
        /// The parser
        /// </summary>
        private readonly IMessageParser parser = null;

        /// <summary>
        /// The wr
        /// </summary>
        private StreamWriter wr = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TxtTraceLog"/> class.
        /// </summary>
        /// <param name="_parser">The _parser.</param>
        public TxtTraceLog(IMessageParser _parser)
        {
            parser = _parser;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (wr == null)
                return;

            wr.Close();
            wr.Dispose();
            wr = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Inserisce un log di un flusso di dati
        /// </summary>
        /// <param name="_currentDevice">Dispositivo locale</param>
        /// <param name="_remoteDevice">Dispositivo remoto</param>
        /// <param name="_data">Dati da storicizzare</param>
        /// <param name="_direction">Direzione dei dati rispetto al Dispositivo locale</param>
        /// <param name="_description">Descrizione aggiuntiva</param>
        /// <param name="_printTypeByteArray">Formattazione di stampa</param>
        public void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction, string _description, PrintTypeByteArray _printTypeByteArray)
        {
            try
            {
                if (wr == null)
                    return;

                var dataStr = "[cannot parse]";
                if (parser != null)
                    dataStr = parser.ParseMessage(_data).Display();
                wr.WriteLine(@"{0}: current device={1}; remote device={2}; data={3}; direction={4}; description={5}; type byte array={6}",
                    DateTime.Now, _currentDevice, _remoteDevice, dataStr, _direction, _description, _printTypeByteArray);
            }
            catch (Exception exc)
            {
                if (wr != null)
                    wr.WriteLine("{0}: Logger exception: {1}", DateTime.Now, exc);
                System.Diagnostics.Debug.Print("{0}: Logger exception: {1}", DateTime.Now, exc);
            }
        }

        /// <summary>
        /// Opens the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Open(string path)
        {
            Close();
            try
            {
                var fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                wr = new StreamWriter(fs);
            }
            catch (Exception exc)
            {
            }
        }

        #endregion Methods
    }
}