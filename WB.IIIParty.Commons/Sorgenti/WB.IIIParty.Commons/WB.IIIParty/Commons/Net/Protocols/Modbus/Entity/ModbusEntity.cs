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
    public interface ModbusEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetId();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void SetId(string id);
        /// <summary>
        /// 
        /// </summary>
        void Start();
        /// <summary>
        /// 
        /// </summary>
        void Stop();
    }
}
