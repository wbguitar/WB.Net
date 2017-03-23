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
    /// 
    /// </summary>
    public class ModbusPoint : IModbusPoint
    {
        #region Private Members

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private string mbPointId;
        /// <summary>
        /// 
        /// </summary>
        private ModbusPointType mbPointType = ModbusPointType.CoilStatus;
        /// <summary>
        /// 
        /// </summary>
        private int mbAddress = 0;
        /// <summary>
        /// 
        /// </summary>
        private int mbSize = 16;
        /// <summary>
        /// 
        /// </summary>
        private int mbExceptionCode = 0;
        /// <summary>
        /// 
        /// </summary>
        private ModbusValueType mbValueType = ModbusValueType.Boolean;
        /// <summary>
        /// 
        /// </summary>
        private ModbusAccessMode mbAccessMode = ModbusAccessMode.Read;
        /// <summary>
        /// 
        /// </summary>
        private List<object> mbPointValue = new List<object>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, object> mbWriteValue = new Dictionary<int, object>();

        #endregion

        #endregion

        #region Public Members

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ModbusPoint() 
        {
            for (int i = 0; i < this.mbSize; i++)
            {
                this.mbPointValue.Add(false);
            }
        }
        /// <summary>
        /// Punto Modbus
        /// </summary>
        /// <param name="pointId">Nome del Punto Modbus</param>
        /// <param name="pointType">Tipo del Punto Modbus</param>
        /// <param name="address">Indirizzo del Punto Modbus</param>
        /// <param name="valType">Tipo del valore del Punto Modbus</param>
        /// <param name="accessMode">Modalità di accesso del Punto Modbus</param>
        public ModbusPoint(string pointId, ModbusPointType pointType, int address, ModbusValueType valType, ModbusAccessMode accessMode)
        {
            this.mbPointId      = pointId;
            this.mbPointType    = pointType;
            this.mbAddress      = address;
            this.mbValueType    = valType;
            this.mbAccessMode   = accessMode;
            switch (valType)
            {
                case ModbusValueType.Boolean:
                    for (int i = 0; i < this.GetMbSize(); i++)
                    {
                        this.mbPointValue.Add(false);
                    }
                    break;
                case ModbusValueType.Float:
                    for (int i = 0; i < this.GetMbSize(); i++)
                    {
                        this.mbPointValue.Add(0.0F);
                    }
                    break;
                case ModbusValueType.Int:
                    for (int i = 0; i < this.GetMbSize(); i++)
                    {
                        this.mbPointValue.Add(0);
                    }
                    break;
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMbPointId()
        {
            return this.mbPointId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointId"></param>
        public void SetMbPointId(string pointId)
        {
            this.mbPointId = pointId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ModbusPointType GetMbPointType()
        {
            return this.mbPointType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointType"></param>
        public void SetMbPointType(ModbusPointType pointType)
        {
            this.mbPointType = pointType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMbAddress()
        {
            return this.mbAddress; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        public void SetMbAddress(int address)
        {
            this.mbAddress = address;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMbSize()
        {
            return this.mbSize; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public void SetMbSize(int size)
        {
            this.mbSize = size;
            this.mbPointValue.Clear();
            switch (this.mbValueType)
            {
                case ModbusValueType.Boolean:
                    for (int i = 0; i < this.mbSize; i++)
                    {
                        this.mbPointValue.Add(false);
                    }
                    break;
                case ModbusValueType.Float:
                    for (int i = 0; i < this.mbSize; i++)
                    {
                        this.mbPointValue.Add(0.0F);
                    }
                    break;
                case ModbusValueType.Int:
                    for (int i = 0; i < this.mbSize; i++)
                    {
                        this.mbPointValue.Add(0);
                    }
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMbExceptionCode()
        {
            return this.mbExceptionCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionCode"></param>
        public void SetMbExceptionCode(int exceptionCode)
        {
            this.mbExceptionCode = exceptionCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ModbusValueType GetMbValueType()
        {
            return this.mbValueType; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueType"></param>
        public void SetMbValueType(ModbusValueType valueType)
        {
            this.mbValueType = valueType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ModbusAccessMode GetMbAccessMode()
        {
            return this.mbAccessMode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessMode"></param>
        public void SetMbAccessMode(ModbusAccessMode accessMode)
        {
            this.mbAccessMode = accessMode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<object> GetMbPointValue()
        {
            return this.mbPointValue; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, object> GetMbWriteValue()
        {
            return this.mbWriteValue;
        }

        #endregion

        #endregion
    }
}
