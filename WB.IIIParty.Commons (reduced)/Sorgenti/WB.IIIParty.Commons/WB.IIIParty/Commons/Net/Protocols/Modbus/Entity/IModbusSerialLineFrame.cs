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
    /// Interfaccia di definizione del frame Modbus seriale
    /// </summary>
    public interface IModbusSerialLineFrame
    {
        /// <summary>
        /// Creazione del frame Modbus
        /// </summary>
        /// <param name="dest">Destinazione</param>
        /// <param name="data">Dati da incapsulare nel frame Modbus</param>
        byte[] CreateFrame(Int16 dest, byte[] data);
        /// <summary>
        /// Analisi del frame Modbus
        /// </summary>
        /// <param name="frameReceived">Frame Modbus ricevuto</param>
        byte[] ParseFrame(byte[] frameReceived);
        /// <summary>
        /// Controllo del frame Modbus
        /// </summary>
        /// <param name="frameReceived">Frame Modbus ricevuto</param>
        /// <returns></returns>
        bool CheckFrame(byte[] frameReceived);
    }
}
