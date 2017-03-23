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

namespace WB.IIIParty.Commons.Net.Protocols.Modbus.Utility
{
    /// <summary>
    /// Generatore di Transaction ID
    /// </summary>
    public class TransactionIDGenerator
    {
        private Int16 transacID;
        /// <summary>
        /// Transaction ID
        /// </summary>
        public Int16 TransacID
        {
            get
            {
                if (transacID < Int16.MaxValue) return transacID++;
                else return 0;
            }
        }

        private static TransactionIDGenerator istance = null;
        /// <summary>
        /// Istanza del generatore di Transaction ID
        /// </summary>
        public static TransactionIDGenerator Istance
        {
            get
            {
                if (istance == null)
                {
                    istance = new TransactionIDGenerator();
                } return istance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected TransactionIDGenerator() { }
    }
}
