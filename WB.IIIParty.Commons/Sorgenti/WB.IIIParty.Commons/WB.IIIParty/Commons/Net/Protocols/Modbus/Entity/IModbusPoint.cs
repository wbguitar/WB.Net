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

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Entity
{
    /// <summary>
    /// Tipi di Punti Modbus
    /// </summary>
    public enum ModbusPointType
    {
        /// <summary>
        /// Coils Status
        /// </summary>
        CoilStatus = 0,
        /// <summary>
        /// Input Status
        /// </summary>
        InputStatus = 1,
        /// <summary>
        /// Input Register
        /// </summary>
        InputRegister = 3,
        /// <summary>
        /// Holding Register
        /// </summary>
        HoldingRegister = 4
    }
    /// <summary>
    /// Tipo dei valori dei Punti Modbus
    /// </summary>
    public enum ModbusValueType
    {
        /// <summary>
        /// Boolean
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// Int16
        /// </summary>
        Int = 1,
        /// <summary>
        /// Float
        /// </summary>
        Float = 2
    }
    /// <summary>
    /// Modalità di accesso ai Punti Modbus
    /// </summary>
    public enum ModbusAccessMode
    {
        /// <summary>
        /// Read
        /// </summary>
        Read = 0,
        /// <summary>
        /// Write
        /// </summary>
        Write = 1,
        /// <summary>
        /// ReadWrite
        /// </summary>
        ReadWrite = 2
    }
    /// <summary>
    /// Interfaccia Modbus Point
    /// </summary>
    public interface IModbusPoint
    {
        /// <summary>
        /// 
        /// </summary>
        string GetMbPointId();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointId"></param>
        void SetMbPointId(string pointId);
        /// <summary>
        /// 
        /// </summary>
        ModbusPointType GetMbPointType();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointType"></param>
        void SetMbPointType(ModbusPointType pointType);
        /// <summary>
        /// 
        /// </summary>
        int GetMbAddress();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        void SetMbAddress(int address);
        /// <summary>
        /// 
        /// </summary>
        int GetMbSize();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<object> GetMbPointValue();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Dictionary<int, object> GetMbWriteValue();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetMbExceptionCode();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        void SetMbSize(int size);
        /// <summary>
        /// 
        /// </summary>
        ModbusValueType GetMbValueType();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueType"></param>
        void SetMbValueType(ModbusValueType valueType);
        /// <summary>
        /// 
        /// </summary>
        ModbusAccessMode GetMbAccessMode();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessMode"></param>
        void SetMbAccessMode(ModbusAccessMode accessMode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionCode"></param>
        void SetMbExceptionCode(int exceptionCode);
        
    }
}
