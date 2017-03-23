// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Papi Rudy
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WB.IIIParty.Commons.Data;

using System.Diagnostics;
using System.Reflection;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Implementa un Logger su un database SqlServer con interfaccia ILog
    /// </summary>
    public class SqlTraceLogger : SqlBaseLogger, ITraceLog
    {        

        #region Field

        private List<object> valueTable = new List<object>();

        private int randomNumber = 0;
        Random randomNumberObject = new Random();
        
        #endregion 

        #region Constructor
        /// <summary>
        /// Costruttore SqlTraceLogger
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        /// <param name="_initialLevel">Livello iniziale del logger</param>
        /// <param name="_config">SqlTraceLoggerConfig</param>
        public SqlTraceLogger(string _name, LogLevels _initialLevel, SqlTraceLoggerConfig _config) : base(_name, _initialLevel, _config, false)
        {
            this.columnsNameList.Add("trace_Ts");
            this.columnsNameList.Add("trace_CurrentDevice");
            this.columnsNameList.Add("trace_RemoteDevice");
            this.columnsNameList.Add("trace_Data");
            this.columnsNameList.Add("trace_Description");
            this.columnsNameList.Add("trace_Direction");
            this.columnsNameList.Add("trace_IncrementalNumber");
        }

        #endregion

        #region ITraceLog Members

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
            //Inserire Enum formattazione dati byte array
            //Stampare la striga del byte array convertita nel seguente modo byte-byte-byte...
            lock (this.thisLock)
            {
                this.valueTable = new List<object>();

                this.valueTable.Add(DateTime.Now);
                this.valueTable.Add(_currentDevice);
                this.valueTable.Add(_remoteDevice);

                
                string byteArrayStr = "";
                for (int i = 0; i < _data.Length; i++)
                {
                    byte ba = _data[i];
                    string baStr = "";
                    switch (_printTypeByteArray)
                    {
                        case PrintTypeByteArray.Ascii:
                            baStr = Convert.ToChar(ba).ToString();
                            break;
                        case PrintTypeByteArray.Hexadecimal:
                            baStr = String.Format("{0:X2} ", ba).Trim();
                            break;
                        case PrintTypeByteArray.Decimal:
                            baStr = String.Format("{0:D} ", ba).Trim();
                            break;
                        default:
                            break;
                    }
                    if (i == _data.Length)
                    {
                        byteArrayStr += baStr;
                    }
                    else
                    {
                        byteArrayStr += baStr + "-";
                    }
                }

                this.valueTable.Add(BitConverter.ToString(_data));
                this.valueTable.Add(_description);
                this.valueTable.Add(((int)_direction).ToString());
                this.randomNumber++;
                this.valueTable.Add(this.randomNumber);

                base.Log(this.valueTable);
            }
        }

        #endregion

    }
}
