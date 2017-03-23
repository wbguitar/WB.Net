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
using System.IO;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using System.Net;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus
{
    /// <summary>
    /// Evento di avvenuta ricezione dati
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="modbusAdu"> Unità ADU Modbus</param>
    public delegate void dDataReceived(IModbusCommunication sender, byte[] modbusAdu);
    /// <summary>
    /// Interfaccia che definisce la comunicazione attraverso il protocollo Modbus.
    /// </summary>
    public interface IModbusCommunication
    {
        /// <summary>
        /// Ritorna lo stato di connessione della comunicazione.
        /// </summary>
        /// <returns>Stato di connessione della comunicazione.</returns>
        bool IsConnected();
        /// <summary>
        /// Ritorna il parametro Modbus Identifier.
        /// </summary>
        /// <returns>Modbus Identifier.</returns>
        string GetId();
        /// <summary>
        /// Imposta il parametro Modbus Identifier.
        /// </summary>
        /// <param name="id">Modbus Identifier.</param>
        void SetId(string id);
        /// <summary>
        /// Avvio della comunicazione.
        /// </summary>
        void StartCommunication();
        /// <summary>
        /// Creazione dell'unità ADU Modbus.
        /// </summary>
        /// <param name="pdu">Unità PDU Modbus</param>
        /// <returns>Unità ADU Modbus</returns>
        byte[] CreateADU(byte[] pdu);
        /// <summary>
        /// Invio dell'unità ADU Modbus.
        /// </summary>
        /// <param name="modbusADU">Unità ADU Modbus</param>
        void SendModbusADU(byte[] modbusADU);
        /// <summary>
        /// Analisi dell'unità ADU Modbus
        /// </summary>
        /// <param name="adu">Unità ADU Modbus</param>
        /// <returns>Unità PDU Modbus</returns>
        byte[] ParseADU(byte[] adu);
        /// <summary>
        /// Arresto della comunicazione.
        /// </summary>
        void StopCommunication();
        /// <summary>
        /// Evento di avvenuta ricezione dati.
        /// </summary>
        event dDataReceived onDataReceived;
    }
}
