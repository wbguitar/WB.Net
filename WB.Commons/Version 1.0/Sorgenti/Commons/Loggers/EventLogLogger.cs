using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace WB.Commons.Loggers
{
    public class EventLogLogger: WB.IIIParty.Commons.Logger.IMessageLog
    {
        EventLog evtLog;

        public EventLogLogger(EventLog _evtLog)
        {
            evtLog = _evtLog;
        }

        //public EventLogLogger(string sourceName, string logName)
        //{
        //    base.Log = logName;
        //    base.Source = sourceName;
        //}

        public bool CanLog(WB.IIIParty.Commons.Logger.LogLevels level)
        {
            return true;
        }

        public void Log(WB.IIIParty.Commons.Logger.LogLevels level, string message)
        {
            System.Diagnostics.EventLogEntryType entryType;
            switch (level)
            {
                case WB.IIIParty.Commons.Logger.LogLevels.Debug:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Disabled:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Error:
                    entryType = System.Diagnostics.EventLogEntryType.Error;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Info:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Trace:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Warning:
                    entryType = System.Diagnostics.EventLogEntryType.Warning;
                    break;
                default:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
            }
            evtLog.WriteEntry(message, entryType);
        }

        public void Log(WB.IIIParty.Commons.Logger.LogLevels level, object caller, string message)
        {

            System.Diagnostics.EventLogEntryType entryType;
            switch (level)
            {
                case WB.IIIParty.Commons.Logger.LogLevels.Debug:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Disabled:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Error:
                    entryType = System.Diagnostics.EventLogEntryType.Error;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Info:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Trace:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
                case WB.IIIParty.Commons.Logger.LogLevels.Warning:
                    entryType = System.Diagnostics.EventLogEntryType.Warning;
                    break;
                default:
                    entryType = System.Diagnostics.EventLogEntryType.Information;
                    break;
            }
            evtLog.WriteEntry(string.Format("Caller: {0}\r\nMessage: {1}", caller, message), entryType);
        }

        public WB.IIIParty.Commons.Logger.LogLevels LogLevel
        {
            get
            {
                return WB.IIIParty.Commons.Logger.LogLevels.Debug;
            }
            set
            {
            }
        }
    }
}
