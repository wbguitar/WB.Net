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
using System.Reflection;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class ModbusSettings
    {
        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private static ModbusSettings instance;
        /// <summary>
        /// 
        /// </summary>
        private System.Collections.Hashtable modbusCommandType = new System.Collections.Hashtable();

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        protected ModbusSettings()
        {

                Assembly mbPDUAssembly = Assembly.GetExecutingAssembly();
                //Assembly mbPDUAssembly = Assembly.LoadFrom("ModbusLib.dll");
                Type mbPDUType;
                Type[] constrParam;
                ConstructorInfo constrInfo;

                mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Net.Protocols.Modbus.PDU.ReadCoils");
                constrParam = new Type[0];
                constrInfo = mbPDUType.GetConstructor(constrParam);
                modbusCommandType.Add(1, constrInfo);

                mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Net.Protocols.Modbus.PDU.ReadInputDiscretes");
                constrParam = new Type[0];
                constrInfo = mbPDUType.GetConstructor(constrParam);
                modbusCommandType.Add(2, constrInfo);

                mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Net.Protocols.Modbus.PDU.ReadHoldingRegisters");
                constrParam = new Type[0];
                constrInfo = mbPDUType.GetConstructor(constrParam);
                modbusCommandType.Add(3, constrInfo);

                mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Net.Protocols.Modbus.PDU.ReadInputRegisters");
                constrParam = new Type[0];
                constrInfo = mbPDUType.GetConstructor(constrParam);
                modbusCommandType.Add(4, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Protocols.Modbus.PDU.WriteSingleCoil");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(5, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Protocols.Modbus.PDU.WriteSingleRegister");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(6, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Protocols.Modbus.PDU.ReadExceptionStatus");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(7, constrInfo);

                mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Net.Protocols.Modbus.PDU.WriteMultipleCoils");
                constrParam = new Type[0];
                constrInfo = mbPDUType.GetConstructor(constrParam);
                modbusCommandType.Add(15, constrInfo);

                mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Net.Protocols.Modbus.PDU.WriteMultipleRegisters");
                constrParam = new Type[0];
                constrInfo = mbPDUType.GetConstructor(constrParam);
                modbusCommandType.Add(16, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Protocols.Modbus.PDU.ReadMultipleFloat");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(33, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Protocols.Modbus.PDU.ReadInputFloat");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(34, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("WB.IIIParty.Commons.Protocols.Modbus.PDU.WriteMultipleFloat");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(36, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("ModbusLib.ReadingEventValues");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(65, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("ModbusLib.ReadingNewEvents");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(66, constrInfo);

                //mbPDUType = mbPDUAssembly.GetType("ModbusLib.LastEventsTrasmitted");
                //constrParam = new Type[0];
                //constrInfo = mbPDUType.GetConstructor(constrParam);
                //modbusCommandType.Add(67, constrInfo);
   
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ModbusSettings GetInstance()
        {
            if (instance == null)
            {
                instance = new ModbusSettings();
            }
            return instance;
        }
        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Hashtable ModbusCommandType
        {
            get { return modbusCommandType; }
        }

        #endregion
    }
}
