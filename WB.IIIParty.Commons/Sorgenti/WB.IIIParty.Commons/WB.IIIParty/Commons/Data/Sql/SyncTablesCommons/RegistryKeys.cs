// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina
//Nome modulo software: SyncTablesCommons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;
 
namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{

    /// <summary>
    /// Permette la lettura di chiavi di registro
    /// </summary>
    public class RegistryKeys : IDisposable
    {
        #region PrivateField
        private IMessageLog log;
        #endregion PrivateField

        #region Const
        //Nomi delle chiavi di registro da leggere

        private const string LocalNameServer = "LocalNameServer";
        private const string LocalLinkedNameServer = "LocalLinkedNameServer";
        private const string RemoteNameServer = "RemoteNameServer";
        private const string RemoteLinkedNameServer = "RemoteLinkedNameServer";
        private const string Password = "Password";
        private const string UserName = "UserName";
        private const string ParamDbName = "ParamDbName";
        private const string ParamSchemaName = "ParamSchemaName";
        private const string ParamTableName = "ParamTableName";
        private const string KeyName = @"SOFTWARE\T&T_MilanoLinea5";
        
        #endregion Const

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        public RegistryKeys()
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
        }
        #endregion Constructor

        #region PublicMethod

        /// <summary>
        /// Restituisce un oggetto ServerInfo che rappresnta
        /// il server locale.
        /// </summary>
        /// <returns></returns>
        public ServerInfo GetLocalServer()
        {
            return this.GetServer(LocalNameServer, LocalLinkedNameServer);
        }

        /// <summary>
        /// Restituisce un oggetto ServerInfo che rappresnta
        /// il server locale.
        /// </summary>
        /// <returns></returns>

        public ServerInfo GetRemoteServer()
        {
            return this.GetServer(RemoteNameServer, RemoteLinkedNameServer);
        }

        /// <summary>
        /// Resituisce il nome del database dei parametri.
        /// </summary>
        /// <returns>Nome del database dei parametri o stringa vuota in
        /// caso di errore.</returns>
        public string GetParamDbName()
        {
            string currentParamDbName = string.Empty;
            try
            {
                RegistryKey Key = Registry.LocalMachine.OpenSubKey(KeyName);
                currentParamDbName = (string)Key.GetValue(ParamDbName);
            }

            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                    " - RegistryKeys.GetParamDbName - " + Ex.Message);
            }
            return currentParamDbName;
        }

        /// <summary>
        /// Resituisce il nome dello schema del database dei parametri.
        /// </summary>
        /// <returns>Nome dello schema o stringa vuota in caso di errore.
        /// </returns>
        public string GetParamSchemaName()
        {
            string currentParamSchemaName = string.Empty;
            try
            {
                RegistryKey Key = Registry.LocalMachine.OpenSubKey(KeyName);
                currentParamSchemaName = (string)Key.GetValue(ParamSchemaName);
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                    "- RegistryKeys.GetParamSchemaName - " + Ex.Message);
            }
            return currentParamSchemaName;
        }


        /// <summary>
        /// Resituisce il nome della tabella dei parametri.
        /// </summary>
        /// <returns>Nome della tabella o stringa vuota in caso di errore.
        /// </returns>
        public string GetParamTableName()
        {
            string currentParamTableName = string.Empty;
            try
            {
                RegistryKey Key = Registry.LocalMachine.OpenSubKey(KeyName);
                currentParamTableName = (string)Key.GetValue(ParamTableName);
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                   " - RegistryKeys.GetParamTableName - " + Ex.Message);
            }
            return currentParamTableName;
        }

        #endregion PublicMethod
        
        #region PrivateMethod

        /// <summary>
        /// Legge le chiavi di registro relative ad un'istanza Sql.
        /// </summary>
        /// <param name="_name">Nome istanza sql</param>
        /// <param name="_linkedName">Nome dell'instanza sql "linked".</param>
        /// <returns></returns>
        private ServerInfo GetServer(string _name, string _linkedName)
        {
            ServerInfo currentServer = new ServerInfo();
            try
            {
                RegistryKey Key = Registry.LocalMachine.OpenSubKey(KeyName);
                currentServer.localName = (string)Key.GetValue(_name);
                currentServer.localLinkedName = (string)Key.GetValue(_linkedName);
                currentServer.userName = (string)Key.GetValue(UserName);
                currentServer.password = (string)Key.GetValue(Password);
            }
            catch (Exception Ex)
            {
                //log
                log.Log(LogLevels.Error,  CustomTimeStamp.GetTimeStamp() +
                    " - RegistryKeys.GetServer - " +Ex.Message);
            }

            return currentServer;
        }
        #endregion PrivateMethod

        #region IDisposable Members

        /// <summary>
        /// Dispose dell'oggetto
        /// </summary>
        public void Dispose()
        {
            if (log != null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion

    }// END CLASS DEFINITION RegistryKeys
}
