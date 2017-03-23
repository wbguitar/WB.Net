// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Diagnostics;
using System.Reflection;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa un Logger su un file con interfaccia NLog
    /// </summary>
    public class NLogTraceLogger : NLogLogger, ITraceLog
    {
        #region Field

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_config">Configurazione</param>
        public NLogTraceLogger(NLogLoggerConfig _config)
            : base(_config)
        {
            
        }

        #endregion
  
        #region Property       

        #endregion


        #region ITraceLog Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_currentDevice"></param>
        /// <param name="_remoteDevice"></param>
        /// <param name="_data"></param>
        /// <param name="_direction"></param>
        /// <param name="_description"></param>
        /// <param name="_printTypeByteArray"></param>
        public void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction, string _description, PrintTypeByteArray _printTypeByteArray)
        {
            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(LogLevels.Trace), base.Config.NLogTargetName, _description);

            entryLog.Properties["TetLoggerName"] = base.Config.Name;
            entryLog.Properties["CurrentDevice"] = _currentDevice;
            entryLog.Properties["RemoteDevice"] = _remoteDevice;
            entryLog.Properties["Direction"] = _direction.ToString();
            entryLog.Properties["Array"] = new ByteArrayFormatter(_data , _printTypeByteArray);

            base.Logger.Log(entryLog);
        }

        #endregion

        #region Internal Class
        
        /// <summary>
        /// 
        /// </summary>
        public class ByteArrayFormatter
        {
            byte[] data;
            PrintTypeByteArray printTypeByteArray;

            public ByteArrayFormatter(byte[] _data, PrintTypeByteArray _printTypeByteArray)
            {
                printTypeByteArray = _printTypeByteArray;
                data = _data;
            }

            public override string ToString()
            {
                StringBuilder byteArrayStr = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    byte b = data[i];

                    switch (printTypeByteArray)
                    {
                        case PrintTypeByteArray.Ascii:
                            byteArrayStr.Append(Convert.ToChar(b));
                            break;
                        case PrintTypeByteArray.Hexadecimal:
                            byteArrayStr.AppendFormat("{0:x2}", b);
                            break;
                        case PrintTypeByteArray.Decimal:
                            byteArrayStr.AppendFormat("{0:D}", b);
                            break;
                        default:
                            break;
                    }

                    if (i < (data.Length-1))
                    {
                        byteArrayStr.Append("-");
                    }
                }

                return byteArrayStr.ToString();
            }
        }

        #endregion
    }
}