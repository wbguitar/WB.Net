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
using System.Collections;
using Microsoft.Win32;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Classe base per la creazione del logger
    /// </summary>
    public abstract class SqlBaseLogger : LoggerConfig, IDisposable
    {
        #region Field
        /// <summary>
        /// Lista dei nomi delle colonne
        /// </summary>
        protected List<string> columnsNameList = new List<string>();
        /// <summary>
        /// Nome database
        /// </summary>
        protected string database;
        /// <summary>
        /// Utente database
        /// </summary>
        protected string userName;
        /// <summary>
        /// Password database
        /// </summary>
        protected string password;
        /// <summary>
        /// Nome Tabella database
        /// </summary>
        protected string tableName;
        /// <summary>
        /// Nome schema database
        /// </summary>
        protected string schema;
        /// <summary>
        /// Livello di loglevel per filtri
        /// </summary>
        protected LogLevels logLevelFilter;
        /// <summary>
        /// CommandToTable per la insert dei log
        /// </summary>
        protected ICommandToTable commandToTable;
        /// <summary>
        /// Oggetto utlizzato per il controllo delle lock
        /// </summary>
        protected Object thisLock = new Object();
        /// <summary>
        /// Oggetto per il controolo delle chiavi di registro e loro stato di cambiamento.
        /// </summary>
        protected RegistryKeyChanged registryKeyChanged;

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        /// <param name="_initialLevel">Livello iniziale del logger</param>
        /// <param name="_config">Base Logger Config</param>
        /// <param name="_activeLogLevelFromRegistry">Base Logger Config</param>
        public SqlBaseLogger(string _name, LogLevels _initialLevel, SqlBaseLoggerConfig _config, bool _activeLogLevelFromRegistry) : base(_name, _initialLevel)
        {
            this.logLevelFilter = _config.InitialLevel;
            this.database = _config.Database;
            this.userName = _config.UserName;
            this.password = _config.Password;
            this.tableName = _config.TableName;
            this.commandToTable = _config.CommandToTable;
            this.schema = _config.Schema;

            //Parametro bool al costruttore che detrmina se è attivo il controllo del LogLevel e se è attivo verifica l'esistenza 
            //della chiave con il logger name, se non esiste la crea e se a false la distrugge.
            if (_activeLogLevelFromRegistry)
            {
                string pathRegistryBase = @"Software\WB.IIIParty";
                string pathRegistry = @"Software\WB.IIIParty\SqlBaseLogger";

                //Controlla l'esistenza della chiave "Software\WB.IIIParty" nel registro, eventualmente crearla.
                RegistryKey RK_app = Registry.LocalMachine.OpenSubKey(pathRegistryBase, true);
                if (RK_app == null)
                {
                    //Creo la chiave WB.IIIParty se non esiste.
                    RK_app = Registry.LocalMachine.OpenSubKey("Software", true).CreateSubKey("WB.IIIParty");
                    RK_app.Close();
                }
                RK_app.Close();
                //Controlla l'esistenza della chiave "Software\WB.IIIParty" nel registro, eventualmente crearla.
                RK_app = Registry.LocalMachine.OpenSubKey(pathRegistry, true);
                if (RK_app == null)
                {
                    //Creo la chiave WB.IIIParty se non esiste.
                    RK_app = Registry.LocalMachine.OpenSubKey(pathRegistryBase, true).CreateSubKey("SqlBaseLogger");
                    RK_app.Close();
                }
                RK_app.Close();
                this.registryKeyChanged = new RegistryKeyChanged(pathRegistry);

                if (_activeLogLevelFromRegistry)
                {
                    //Se non esiste il valore viene creato.
                    RK_app = Registry.LocalMachine.OpenSubKey(pathRegistry);
                    string[] names = RK_app.GetValueNames();

                    if (!names.Contains<string>(this.Name))
                    {
                        RK_app = Registry.LocalMachine.OpenSubKey(pathRegistry, true);
                        RK_app.SetValue(this.Name, (int)_initialLevel);
                        RK_app.Close();
                    }
                    this.registryKeyChanged.AddNotify(SetLogLevelFromRegistryKey);
                }
                else
                {
                    //Se esiste il valore viene cancellato.
                    RK_app = Registry.LocalMachine.OpenSubKey(pathRegistry);
                    string[] names = RK_app.GetValueNames();

                    if (names.Contains<string>(this.Name))
                    {
                        RK_app = Registry.LocalMachine.OpenSubKey(pathRegistry, true);
                        RK_app.DeleteValue(this.Name);
                        RK_app.Close();
                    }
                    this.registryKeyChanged.RemoveNotify(SetLogLevelFromRegistryKey);
                }
            }
        }

        #endregion

        #region Protected Method
        /// <summary>
        /// Inserisce un log di un flusso di dati
        /// </summary>
        /// <param name="_valueTable">Lista valori da passare per la insert</param>
        protected void Log(List<object> _valueTable)
        {            
            try
            {
                System.Threading.Thread threadExecute = System.Threading.Thread.CurrentThread;

                //Called Method
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase method = stackFrame.GetMethod();
                string methodName = method.Name;

                this.commandToTable.Insert(this.database, this.schema, this.tableName, this.columnsNameList, _valueTable);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("DB Name = " + this.database
                    + "DB Name = " + this.database
                    + "DB Schema = " + this.schema
                    + "DB TableName = " + this.tableName
                    + "Count colonne = " + this.columnsNameList.Count.ToString()
                    + "Count Valori = " + _valueTable.ToString()
                    + " - " + ex.ToString());
            }            
        }
        /// <summary>
        /// Riceve la notifica di cambiamento delle chiave di regsitro del LogLevel di un logger.
        /// </summary>
        protected void SetLogLevelFromRegistryKey(Hashtable keyC)
        {
            if (keyC.ContainsKey(this.Name))
            {
                this.logLevelFilter = (LogLevels)keyC[this.Name];
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if(this.registryKeyChanged!=null) this.registryKeyChanged.Dispose();
        }

        #endregion
    }
}
