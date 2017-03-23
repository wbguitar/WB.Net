using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace WB.IIIParty.Commons.Logger
{
    /// <summary>
    /// Summary description for RegistryKeyChanged.
    /// </summary>
    public class RegistryKeyChanged : IDisposable
    {
        #region Constant
        //setting registry check
        const uint WAIT_OBJECT_0 = 0;
        const long HKEY_LOCAL_MACHINE = 0x80000002L;
        const long REG_NOTIFY_CHANGE_LAST_SET = 0x00000004L;
        #endregion

        #region DLL Import
        [DllImport("kernel32.dll", EntryPoint = "CreateEvent")]
        static extern IntPtr CreateEvent(IntPtr eventAttributes,
            bool manualReset, bool initialState, String name);

        [DllImport("advapi32.dll", EntryPoint = "RegOpenKey")]
        static extern IntPtr RegOpenKey(IntPtr key, String subKey,
            out IntPtr resultSubKey);

        [DllImport("advapi32.dll", EntryPoint = "RegNotifyChangeKeyValue")]
        static extern long RegNotifyChangeKeyValue(IntPtr key,
            bool watchSubTree, int notifyFilter, IntPtr regEvent, bool
            async);

        [DllImport("kernel32.dll", EntryPoint = "WaitForSingleObject")]
        static extern uint WaitForSingleObject(IntPtr handle, int timeOut);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        static extern IntPtr CloseHandle(IntPtr handle);

        #endregion

        #region Private Fields

        private RegistryKey RK_app;
        Thread RegCheck;
        string mKey;        
        ArrayList mDelegateList = new ArrayList();
        /// <summary>
        /// Delegato stato Key.
        /// </summary>
        /// <param name="values"></param>
        public delegate void dKEyValues(Hashtable values);

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="key"></param>
        public RegistryKeyChanged(string key)
        {
            //
            // TODO: Add constructor logic here
            //
            //registry value change metod
            this.mKey = key;

            RegCheck = new Thread(new ThreadStart(regcheck));
            RegCheck.Start();
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Attiva controllo stato chiave.
        /// </summary>
        /// <param name="deleg"></param>
        public void AddNotify(dKEyValues deleg)
        {
            lock (this.mDelegateList.SyncRoot)
            {
                this.mDelegateList.Add(deleg);
            }

            this.ReadRegistryValues();
        }
        /// <summary>
        /// Disattiva controllo stato chiave.
        /// </summary>
        /// <param name="deleg"></param>
        public void RemoveNotify(dKEyValues deleg)
        {
            lock (this.mDelegateList.SyncRoot)
            {
                this.mDelegateList.Remove(deleg);
            }
        }


        #endregion

        #region Private Method
        private void ReadRegistryValues()
        {
            try
            {
                Hashtable values = new Hashtable();
                //ricavo i valori di impostazione dal registro di sistema
                //se non esistono vengono creati automaticamente
                //Key
                RK_app = Registry.LocalMachine.OpenSubKey(this.mKey, true);
                //parametro port
                string[] names = RK_app.GetValueNames();

                foreach (string name in names)
                {
                    values.Add(name, RK_app.GetValue(name));
                }
                lock (this.mDelegateList.SyncRoot)
                {

                    foreach (dKEyValues del in this.mDelegateList)
                    {
                        del(values);
                    }

                }
            }
            catch (Exception )
            {
                //CGUtils.logServer.LogMessage(CGUtils.logServer.FULL, ex.Message, "RegistryKeyChangedClass");
            }
        }

        private void regcheck()
        {
            try
            {
                string key;

                IntPtr myKey;

                key = this.mKey;
                unchecked
                {
                    RegOpenKey(new IntPtr((int)HKEY_LOCAL_MACHINE), key, out myKey);
                }

                do
                {
                    try
                    {
                        IntPtr myEvent = CreateEvent((IntPtr)null, false, false, null);
                        RegNotifyChangeKeyValue(myKey, true, (int)REG_NOTIFY_CHANGE_LAST_SET, myEvent, true);
                        if ((WaitForSingleObject(myEvent, 5000) == WAIT_OBJECT_0))
                        {
                            this.ReadRegistryValues();
                        }
                        CloseHandle(myEvent);
                    }
                    catch (Exception ex)
                    {
                        if(ex is ThreadAbortException) throw ex;
                        //CGUtils.logServer.LogMessage(CGUtils.logServer.FULL, ex.Message, "RegistryKeyChangedClass");
                    }
                }
                while (true);
            }
            catch (ThreadAbortException tex)
            {

            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // TODO:  Add RegistryKeyChanged.Dispose implementation
            try
            {
                this.RegCheck.Abort();

                this.RK_app.Close();
            }
            catch (Exception )
            {
                //CGUtils.logServer.LogMessage(CGUtils.logServer.FULL, ex.Message, "RegistryKeyChangedClass");
            }
        }

        #endregion
    }
}
