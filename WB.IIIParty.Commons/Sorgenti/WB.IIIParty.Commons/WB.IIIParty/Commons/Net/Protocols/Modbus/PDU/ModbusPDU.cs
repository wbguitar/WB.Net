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
using System.Text;
using WB.IIIParty.Commons.Net.Protocols.Modbus.Entity;

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.PDU
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ModbusPDU
    {
        /// <summary>
        ///  Creazione di una richiesta Modbus PDU.  
        /// </summary>
        /// <param name="point">Punto Modbus</param>
        /// <returns>Richiesta serializzata</returns>
        public abstract byte[] MbCreateReqPDU(IModbusPoint point);
        /// <summary>
        /// Parsing di una risposta Modbus PDU.
        /// </summary>
        /// <param name="requestData">Richiesta serializzata</param>
        /// <param name="point">Punto Modbus</param>
        public abstract void MbParseReqPDU(byte[] requestData, ref IModbusPoint point);
        /// <summary>
        ///  Creazione di una risposta Modbus PDU. 
        /// </summary>
        /// <param name="point">Punto Modbus</param>
        /// <returns>Risposta serializzata</returns>
        public abstract byte[] MbCreateRspPDU(IModbusPoint point);
        /// <summary>
        ///  Parsing di una risposta Modbus PDU.
        /// </summary>
        /// <param name="responseData">Risposta serializzata</param>
        /// /// <param name="point">Punto Modbus</param>
        public abstract void MbParseRspPDU(byte[] responseData, ref IModbusPoint point);
    }
}
