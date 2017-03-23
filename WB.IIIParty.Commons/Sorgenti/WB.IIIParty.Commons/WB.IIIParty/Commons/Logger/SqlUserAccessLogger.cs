// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Papi Rudy
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2012-10-09 15:28:09 +0200 (mar, 09 ott 2012) $
//Versione: $Rev: 114 $
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
    public class SqlUserAccessLogger : SqlBaseLogger, IUserAccessLog
    {
        #region Field

        private List<object> valueTable = new List<object>();

        #endregion 

        #region Constructor
        /// <summary>
        /// Costruttore SqlTraceLogger
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        /// <param name="_initialLevel">Livello iniziale del logger</param>
        /// <param name="_config">SqlTraceLoggerConfig</param>
        /// <param name="_activeLogLevelFromRegistry">Abilita la possibilità di modificare il livello di log da registro</param>
        public SqlUserAccessLogger(string _name, LogLevels _initialLevel, SqlUserAccessLoggerConfig _config, bool _activeLogLevelFromRegistry)
            : base(_name, _initialLevel, _config, _activeLogLevelFromRegistry)
        {
            this.columnsNameList.Add("uah_Ts");
            this.columnsNameList.Add("uah_UserName");
            this.columnsNameList.Add("uah_LevelId");
            this.columnsNameList.Add("uah_Esito");
            this.columnsNameList.Add("uah_SubsystemType");
            this.columnsNameList.Add("uah_WorkstationId");
        }

        #endregion

        #region Private Method

        private string GetResultDescription(UserAccessResults result)
        {
            switch (result)
            {
                case UserAccessResults.LogInAdministratorAlreadyLogged:
                    {
                        return "Un altro Amministratore già autenticato";
                    }
                case UserAccessResults.LogInException:
                    {
                        return "Eccezione durante l'esecuzione del LogIn";
                    }
                case UserAccessResults.LogInOk:
                    {
                        return "LogIn eseguito";
                    }
                case UserAccessResults.LogInPasswordError:
                    {
                        return "Password errata";
                    }
                case UserAccessResults.LogInUserAlreadyLogger:
                    {
                        return "Utente già autenticato";
                    }
                case UserAccessResults.LogInUserError:
                    {
                        return "Utente errato";
                    }
                case UserAccessResults.LogOutException:
                    {
                        return "Eccezione durante l'esecuzione del LogOut";
                    }
                case UserAccessResults.LogOutOk:
                    {
                        return "LogOut eseguito";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        #endregion

        #region IUserAccessLog Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="group"></param>
        /// <param name="workstationInfo"></param>
        /// <param name="applicationInfo"></param>
        /// <param name="result"></param>
        public void Log(string userName, int group, string workstationInfo, string applicationInfo, UserAccessResults result)
        {
            lock (this.thisLock)
            {
                this.valueTable = new List<object>();

                this.valueTable.Add(DateTime.Now);
                this.valueTable.Add(userName);
                this.valueTable.Add(group);                                
                this.valueTable.Add(GetResultDescription(result));
                this.valueTable.Add(applicationInfo);
                this.valueTable.Add(workstationInfo);

                base.Log(this.valueTable);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="group"></param>
        /// <param name="workstationInfo"></param>
        /// <param name="applicationInfo"></param>
        /// <param name="result"></param>
        public void Log(string userName, string group, string workstationInfo, string applicationInfo, UserAccessResults result)
        {
            lock (this.thisLock)
            {
                this.valueTable = new List<object>();

                this.valueTable.Add(DateTime.Now);
                this.valueTable.Add(userName);
                this.valueTable.Add(group);
                this.valueTable.Add(GetResultDescription(result));
                this.valueTable.Add(applicationInfo);
                this.valueTable.Add(workstationInfo);

                base.Log(this.valueTable);
            }
        }
        #endregion
    }
}
