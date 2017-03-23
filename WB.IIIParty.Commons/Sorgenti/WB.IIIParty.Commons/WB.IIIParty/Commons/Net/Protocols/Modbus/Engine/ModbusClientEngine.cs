// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Bernini Francesco
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WB.IIIParty.Commons.Net.Protocols.Modbus;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;
using WB.IIIParty.Commons.Logger;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Engine
{
    /// <summary>
    /// Lista di Client Modbus
    /// </summary>
    public enum ModbusClientType
    {
        /// <summary>
        /// Comunicazione seriale.
        /// </summary>
        Serial = 0,
        /// <summary>
        /// Comunicazione TCP/IP.
        /// </summary>
        TcpIp = 1
    }
    /// <summary>
    /// Motore per Client Modbus
    /// </summary>
    public class ModbusClientEngine
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private List<ModbusTcpIpClient> mbTcpIpClientList;
        /// <summary>
        /// 
        /// </summary>
        private ModbusClientType mbClientType;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, ModbusMaster> mbMasterList = new Dictionary<string, ModbusMaster>();
        /// <summary>
        /// 
        /// </summary>
        private IModbusCommunication mbCommunicationDriver;
        /// <summary>
        /// 
        /// </summary>
        private bool isStarted;
        /// <summary>
        /// Oggetto per la creazione dei messaggi di log.
        /// </summary>
        private IMessageLog messageLog;

        #endregion

        #region Private Method

        /// <summary>
        /// 
        /// </summary>
        private void SettingMbCliTcpIp()
        {
            try
            {
                for (int i = 0; i < this.mbTcpIpClientList.Count; i++)
                {
                    this.mbCommunicationDriver = new ModbusTCPIPClientCommunication(
                        this.mbTcpIpClientList[i].ServerIpAddress.ToString(),
                        this.mbTcpIpClientList[i].ServerPort,
                        (short)this.mbTcpIpClientList[i].DeviceId,
                        this.mbTcpIpClientList[i].ReconnectScanTime,
                        this.mbTcpIpClientList[i].ConnectionDelay);
                    ((ModbusTCPIPClientCommunication)this.mbCommunicationDriver).onConnect += new ModbusTCPIPClientCommunication.dOnConnect(ModbusClientEngine_onConnect);
                    ((ModbusTCPIPClientCommunication)this.mbCommunicationDriver).onConnectFailure += new ModbusTCPIPClientCommunication.dConnectFailure(ModbusClientEngine_onConnectFailure);
                    ((ModbusTCPIPClientCommunication)this.mbCommunicationDriver).onDisconnect += new ModbusTCPIPClientCommunication.dOnDisconnect(ModbusClientEngine_onDisconnect);
                    ((ModbusTCPIPClientCommunication)this.mbCommunicationDriver).onTrace += new ModbusTCPIPClientCommunication.dOnTrace(ModbusClientEngine_onTrace);
                    ((ModbusTCPIPClientCommunication)this.mbCommunicationDriver).onDataReceived += new dDataReceived(ModbusClientEngine_onDataReceived);
                    this.mbCommunicationDriver.SetId(this.mbTcpIpClientList[i].ClientName);

                    ModbusMaster mbMasterTcpIp = new ModbusMaster(this.mbCommunicationDriver,
                        this.mbTcpIpClientList[i].MbPointList,
                        this.mbTcpIpClientList[i].ScanTime,
                        this.mbTcpIpClientList[i].TimeOut, this.messageLog);
                    mbMasterTcpIp.SetId(this.mbTcpIpClientList[i].ClientName);

                    this.mbMasterList.Add(mbMasterTcpIp.GetId(), mbMasterTcpIp);
                    mbMasterTcpIp.MessageLog = this.messageLog;

                    mbMasterTcpIp.onResponseReceived += new ModbusMaster.dResponseReceived(mbMasterTcpIp_onResponseReceived);
                    mbMasterTcpIp.onValueChanged += new ModbusMaster.dValueChanged(mbMasterTcpIp_onValueChanged);

                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mbPointChanged"></param>
        /// <param name="indexValueChanged"></param>
        void mbMasterTcpIp_onValueChanged(ModbusMaster sender, IModbusPoint mbPointChanged, int indexValueChanged)
        {
            try
            {
                if (this.onValueChanged != null)
                {
                    this.onValueChanged(sender, mbPointChanged, indexValueChanged);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mbPointResponse"></param>
        private void mbMasterTcpIp_onResponseReceived(ModbusMaster sender, IModbusPoint mbPointResponse)
        {
            try
            {
                if (this.onResponseReceived != null)
                {
                    this.onResponseReceived(sender, mbPointResponse);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="modbusAdu"></param>
        private void ModbusClientEngine_onDataReceived(IModbusCommunication sender, byte[] modbusAdu)
        {
            try
            {
                if (this.onDataReceived != null)
                {
                    this.onDataReceived(sender, modbusAdu);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        void ModbusClientEngine_onTrace(WB.IIIParty.Commons.Protocol.IStream sender, WB.IIIParty.Commons.Protocol.TraceDirections direction, byte[] data, string description)
        {
            try
            {
                if (this.onTrace != null)
                {
                    this.onTrace(sender, direction, data, description);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ModbusClientEngine_onDisconnect(IModbusCommunication sender)
        {
            try
            {
                if (this.onDisconnect != null)
                {
                    this.onDisconnect((ModbusTCPIPClientCommunication)sender);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ModbusClientEngine_onConnectFailure(ModbusTCPIPClientCommunication sender)
        {
            try
            {
                if (this.onConnectFailure != null)
                {
                    this.onConnectFailure(sender);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ModbusClientEngine_onConnect(IModbusCommunication sender)
        {
            try
            {
                if (this.onConnect != null)
                {
                    this.onConnect((ModbusTCPIPClientCommunication)sender);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// Metodo di utilizzo di messageLog.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logMessage"></param>
        private void Log(LogLevels logLevel, string logMessage)
        {
            if (this.messageLog != null)
            {
                this.messageLog.Log(logLevel, this, logMessage);
            }
        }

        #endregion

        #endregion

        #region Public Members

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mbTcpIpCliList"></param>
        /// <param name="mbCliType"></param>
        /// <param name="msgLog"></param>
        public ModbusClientEngine(List<ModbusTcpIpClient> mbTcpIpCliList, ModbusClientType mbCliType, IMessageLog msgLog)
        {
            this.mbClientType = mbCliType;
            this.messageLog = msgLog;
            this.mbTcpIpClientList = mbTcpIpCliList;

            switch (this.mbClientType)
            {
                case ModbusClientType.TcpIp:
                    this.SettingMbCliTcpIp();
                    break;
                case ModbusClientType.Serial:
                    break;

            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 
        /// </summary>
        public void StartEngine()
        {
            try
            {
                foreach (string key in this.mbMasterList.Keys)
                {
                    WB.IIIParty.Commons.Net.Protocols.Modbus.Entity.ModbusMaster mbMasterTcpIp = this.mbMasterList[key];
                    mbMasterTcpIp.Start();
                }
                this.isStarted = true;
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void StopEngine()
        {
            try
            {
                foreach (string key in this.mbMasterList.Keys)
                {
                    WB.IIIParty.Commons.Net.Protocols.Modbus.Entity.ModbusMaster mbMasterTcpIp = this.mbMasterList[key];
                    mbMasterTcpIp.Stop();
                }
                this.isStarted = false;
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mbTcpIpClientName"></param>
        /// <param name="mbPoint"></param>
        public void UpdateValue(string mbTcpIpClientName, IModbusPoint mbPoint)
        {
            try
            {
                if (this.mbMasterList.ContainsKey(mbTcpIpClientName))
                {
                    WB.IIIParty.Commons.Net.Protocols.Modbus.Entity.ModbusMaster mbMasterTcpIp = this.mbMasterList[mbTcpIpClientName];
                    mbMasterTcpIp.SendWriteRequest(mbPoint);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusClientEngine: - " + e.ToString());
            }
        }

        #endregion

        #region Public Events & Delegates

        /// <summary>
        /// 
        /// </summary>
        public delegate void dConnectFailure(ModbusTCPIPClientCommunication sender);
        /// <summary>
        /// 
        /// </summary>
        public event dConnectFailure onConnectFailure;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mbPointResponse"></param>
        public delegate void dResponseReceived(ModbusMaster sender, IModbusPoint mbPointResponse);
        /// <summary>
        /// 
        /// </summary>
        public event dResponseReceived onResponseReceived;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mbPointResponse"></param>
        /// <param name="indexValueChanged"></param>
        public delegate void dValueChanged(ModbusMaster sender, IModbusPoint mbPointResponse, int indexValueChanged);
        /// <summary>
        /// 
        /// </summary>
        public event dValueChanged onValueChanged;
        /// <summary>
        /// 
        /// </summary>
        public event dDataReceived onDataReceived;
        /// <summary>
        /// 
        /// </summary>
        public delegate void dOnConnect(ModbusTCPIPClientCommunication sender);
        /// <summary>
        /// 
        /// </summary>
        public event dOnConnect onConnect;
        /// <summary>
        /// 
        /// </summary>
        public delegate void dOnDisconnect(ModbusTCPIPClientCommunication sender);
        /// <summary>
        /// 
        /// </summary>
        public event dOnDisconnect onDisconnect;
        /// <summary>
        /// 
        /// </summary>
        public delegate void dOnTrace(WB.IIIParty.Commons.Protocol.IStream sender, WB.IIIParty.Commons.Protocol.TraceDirections direction, byte[] data, string description);
        /// <summary>
        /// 
        /// </summary>
        public event dOnTrace onTrace;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsStarted
        {
            get { return this.isStarted; }
        }

        #endregion

        #endregion
    }
}
