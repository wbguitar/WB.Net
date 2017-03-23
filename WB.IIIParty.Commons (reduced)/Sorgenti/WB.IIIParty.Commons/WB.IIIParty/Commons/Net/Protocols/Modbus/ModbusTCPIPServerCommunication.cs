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

namespace WB.IIIParty.Commons.Net.Protocols.Modbus
{
    /// <summary>
    /// 
    /// </summary>
    public class ModbusTCPIPServerCommunication : IModbusCommunication
    {
        /// <summary>
        /// Ritorna lo stato di connessione della comunicazione.
        /// </summary>
        /// <returns>Stato di connessione della comunicazione.</returns>
        public virtual bool IsConnected()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Ritorna il parametro Modbus Identifier.
        /// </summary>
        /// <returns>Modbus Identifier.</returns>
        public virtual string GetId()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Imposta il parametro Modbus Identifier.
        /// </summary>
        /// <param name="id">Modbus Identifier.</param>
        public virtual void SetId(string id)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Avvio della comunicazione.
        /// </summary>
        public virtual void StartCommunication()
        {
        }
        /// <summary>
        /// Creazione dell'unità ADU Modbus.
        /// </summary>
        /// <param name="pdu">Unità PDU Modbus</param>
        /// <returns>Unità ADU Modbus</returns>
        public virtual byte[] CreateADU(byte[] pdu)
        {
            byte[] result = new byte[1];
            return result;
        }
        /// <summary>
        /// Invio dell'unità ADU Modbus.
        /// </summary>
        /// <param name="modbusADU">Unità ADU Modbus</param>
        public virtual void SendModbusADU(byte[] modbusADU)
        {
        }
        /// <summary>
        /// Analisi dell'unità ADU Modbus
        /// </summary>
        /// <param name="adu">Unità ADU Modbus</param>
        /// <returns>Unità PDU Modbus</returns>
        public virtual byte[] ParseADU(byte[] adu)
        {
            byte[] result = new byte[1];
            return result;
        }
        /// <summary>
        /// Arresto della comunicazione.
        /// </summary>
        public virtual void StopCommunication()
        {
        }
        /// <summary>
        /// Evento di avvenuta ricezione dati.
        /// </summary>
        public event dDataReceived onDataReceived;
    }
}
