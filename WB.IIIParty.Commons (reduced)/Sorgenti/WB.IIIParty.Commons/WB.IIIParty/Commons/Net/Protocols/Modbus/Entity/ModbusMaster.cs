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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;
using WB.IIIParty.Commons.Net.Protocols.Modbus;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class ModbusMaster : ModbusEntity
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private string mbClientId;
        /// <summary>
        /// 
        /// </summary>
        private IModbusCommunication                mbCommunicationDriver;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, IModbusPoint>     mbPointList;
        /// <summary>
        /// 
        /// </summary>
        private List<ModbusRequest>                 mbRequestList;
        /// <summary>
        /// 
        /// </summary>
        private TimeSpan                            mbScantime;
        /// <summary>
        /// 
        /// </summary>
        private TimeSpan                            mbTimeout;
        /// <summary>
        /// 
        /// </summary>
        private Thread                              scannigThread;
        /// <summary>
        /// 
        /// </summary>
        private int                                 lastRequestSent;
        /// <summary>
        /// 
        /// </summary>
        private AutoResetEvent                      autoResetEvent;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Int16, ModbusRequest>    transactionTable;
        /// <summary>
        /// Oggetto per la creazione dei messaggi di log.
        /// </summary>
        private IMessageLog                         messageLog;
        /// <summary>
        /// 
        /// </summary>
        private bool onScan = false;

        #endregion

        #region Private Method

        /// <summary>
        /// 
        /// </summary>
        private void Scan()
        {
            try
            {
                //  Definizione dei parametri per catturare l'ora corrente.
                DateTime startTime;
                DateTime stopTime;
                TimeSpan timeBySpend;
                double diffTime;

                int iTimeout = (int)this.mbTimeout.TotalMilliseconds;

                while (this.onScan)
                {
                    //  Tempo di inizio lettura.
                    startTime = DateTime.Now;

                    if (this.mbCommunicationDriver.IsConnected())
                    {
                        try
                        {
                            if (this.mbRequestList.Count > 0)
                            {
                                byte[] mbReadRequestPDU = this.mbRequestList[this.lastRequestSent].ReadRequest;
                                if (mbReadRequestPDU.Length > 1)
                                {
                                    this.SendReadRequest(mbReadRequestPDU);
                                    if (!this.autoResetEvent.WaitOne(iTimeout, false))
                                    {
                                        // Time Out scaduto in ricezione della risposta
                                        this.Log(LogLevels.Error, "ModbusMaster: - Read request expired timeout ");
                                    }
                                }
                                this.lastRequestSent = (this.lastRequestSent + 1) % this.mbRequestList.Count;
                            }
                        }
                        catch (Exception e)
                        {
                            this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
                        }
                    }
                    stopTime = DateTime.Now;
                    timeBySpend = stopTime - startTime;
                    diffTime = timeBySpend.TotalMilliseconds;
                    int iScantime = (int)this.mbScantime.TotalMilliseconds;
                    if ((diffTime >= 0) && (diffTime < iScantime))
                    {
                        //  Attende in base allo Scan Time.
                        Thread.Sleep(iScantime - Convert.ToInt32(diffTime));
                    }
                    else
                    {
                        Thread.Sleep(iScantime);
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SendReadRequest(byte[] mbRequestPDU)
        {
            try
            {
                if (mbRequestPDU.Length > 1)
                {
                    byte[] mbRequestADU = this.mbCommunicationDriver.CreateADU(mbRequestPDU);
                    byte[] _tID = new byte[2];
                    _tID[0] = mbRequestADU[1];
                    _tID[1] = mbRequestADU[0];
                    Int16 tID = BitConverter.ToInt16(_tID, 0);
                    if (this.transactionTable.ContainsKey(tID))
                    {
                        this.transactionTable.Remove(tID);
                    }
                    this.transactionTable.Add(tID, this.mbRequestList[this.lastRequestSent]);
                    this.mbCommunicationDriver.SendModbusADU(mbRequestADU);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="modbusAdu"></param>
        private void mbCommunicationDriver_onDataReceived(IModbusCommunication sender, byte[] modbusAdu)
        {
            try
            {
                if (modbusAdu.Length > 0)
                {
                    byte[] tId = new byte[2];
                    tId[0] = modbusAdu[1];
                    tId[1] = modbusAdu[0];

                    Int16 transactionId = BitConverter.ToInt16(tId, 0);

                    if (this.transactionTable.ContainsKey(transactionId))
                    {
                        byte[] modbusPdu = this.mbCommunicationDriver.ParseADU(modbusAdu);

                        ModbusRequest mbRequest = this.transactionTable[transactionId];
                        IModbusPoint mbPointReq = mbRequest.MbPoint;
                        List<Object> oldPointValue = new List<object>();
                        for (int i = 0; i < mbPointReq.GetMbPointValue().Count; i++)
                        {
                            oldPointValue.Add(mbPointReq.GetMbPointValue()[i]);
                        }

                        ModbusResponse mbRespose = new ModbusResponse(modbusPdu, mbPointReq, this.messageLog);
                        IModbusPoint mbPointRsp = mbRespose.MbPoint;
                        List<Object> newPointValue = mbRespose.MbPoint.GetMbPointValue();

                        if (mbPointReq.GetMbValueType() == mbPointReq.GetMbValueType())
                        {
                            switch (mbPointReq.GetMbValueType())
                            {
                                case ModbusValueType.Boolean:
                                    for (int i = 0; i < oldPointValue.Count; i++)
                                    {

                                        if ((bool)(oldPointValue[i]) != (bool)(newPointValue[i]))
                                        {
                                            if (this.onValueChanged != null)
                                            {
                                                this.onValueChanged(this, mbPointRsp, i);
                                            }
                                        }
                                    }
                                    break;
                                case ModbusValueType.Float:
                                    for (int i = 0; i < oldPointValue.Count; i++)
                                    {

                                        if ((float)(oldPointValue[i]) != (float)(newPointValue[i]))
                                        {
                                            if (this.onValueChanged != null)
                                            {
                                                this.onValueChanged(this, mbPointRsp, i);
                                            }
                                        }
                                    }
                                    break;
                                case ModbusValueType.Int:
                                    for (int i = 0; i < oldPointValue.Count; i++)
                                    {

                                        if ((short)(oldPointValue[i]) != (short)(newPointValue[i]))
                                        {
                                            if (this.onValueChanged != null)
                                            {
                                                this.onValueChanged(this, mbPointRsp, i);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (mbPointRsp.GetMbExceptionCode() == 0)
                        {
                            if (this.onResponseReceived != null)
                            {
                                this.onResponseReceived(this, mbPointRsp);
                            }
                        }
                    }
                    this.autoResetEvent.Set();
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
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

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modbusDriver"></param>
        /// <param name="pointList"></param>
        /// <param name="scantime"></param>
        /// <param name="timeout"></param>
        /// <param name="msgLog"></param>
        public ModbusMaster(IModbusCommunication modbusDriver, Dictionary<string, IModbusPoint> pointList, TimeSpan scantime, TimeSpan timeout, IMessageLog msgLog)
        {
            this.messageLog = msgLog;
            this.mbCommunicationDriver = modbusDriver;
            this.mbCommunicationDriver.onDataReceived += new dDataReceived(mbCommunicationDriver_onDataReceived);
            this.mbPointList = pointList;
            this.mbRequestList = new List<ModbusRequest>();
            foreach (string key in this.mbPointList.Keys)
            {
                ModbusRequest mbRequest = new ModbusRequest(mbPointList[key], this.messageLog);
                this.mbRequestList.Add(mbRequest);
            }
            this.mbScantime = scantime;
            this.mbTimeout = timeout;
            this.lastRequestSent = 0;
            this.autoResetEvent = new System.Threading.AutoResetEvent(false);
            this.transactionTable = new Dictionary<short, ModbusRequest>();
        }

        #endregion

        #region Public Method
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetId()
        {
            return this.mbClientId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public virtual void SetId(string id)
        {
            this.mbClientId = id;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Start()
        {
            try
            {
                this.mbCommunicationDriver.StartCommunication();
                this.onScan = true;
                this.scannigThread = new Thread(new ThreadStart(this.Scan));
                this.scannigThread.Start();
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                if (this.scannigThread.IsAlive)
                {
                    this.onScan = false;
                    this.scannigThread.Join();
                    this.scannigThread.Abort();
                }

                this.mbCommunicationDriver.StopCommunication();
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void SendWriteRequest(IModbusPoint mbPoint)
        {
            try
            {
                ModbusRequest mbRequest = new ModbusRequest(mbPoint, this.messageLog);
                byte[] mbRequestPDU = mbRequest.WriteRequest;
                if (mbRequestPDU.Length > 1)
                {
                    byte[] mbRequestADU = this.mbCommunicationDriver.CreateADU(mbRequestPDU);
                    byte[] _tID = new byte[2];
                    _tID[0] = mbRequestADU[1];
                    _tID[1] = mbRequestADU[0];
                    Int16 tID = BitConverter.ToInt16(_tID, 0);
                    if (this.transactionTable.ContainsKey(tID))
                    {
                        this.transactionTable.Remove(tID);
                    }
                    this.transactionTable.Add(tID, this.mbRequestList[this.lastRequestSent]);
                    this.mbCommunicationDriver.SendModbusADU(mbRequestADU);
                }
            }
            catch (Exception e)
            {
                this.Log(LogLevels.Error, "ModbusMaster: - " + e.ToString());
            }
        }

        #endregion
        
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
        /// <param name="mbPointChanged"></param>
        /// <param name="indexValueChanged"></param>
        public delegate void dValueChanged(ModbusMaster sender, IModbusPoint mbPointChanged, int indexValueChanged);
        /// <summary>
        /// 
        /// </summary>
        public event dValueChanged onValueChanged;

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, IModbusPoint> MbPointList
        {
            get { return this.mbPointList; }
        }
        /// <summary>
        /// 
        /// </summary>
        public IMessageLog MessageLog
        {
            set 
            { 
                this.messageLog = value; 
            }
        }

        #endregion

        #endregion
    }
}
