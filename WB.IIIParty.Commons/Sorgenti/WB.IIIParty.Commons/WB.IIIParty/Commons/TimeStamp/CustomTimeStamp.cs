// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WB.IIIParty.Commons.TimeStamp
{
    /// <summary>
    /// Data -  ora 
    /// </summary>
    public class CustomTimeStamp
    {

        #region PrivateField
        
        private static string TimeStampFormat = "dd/MM/yyyy HH:mm:ss.fff";

        #endregion PrivateField

        #region PublicField
        
        #endregion PublicField
                
        #region PublicMethod

        /// <summary>
        /// Data-ora attuale in UTC.
        /// </summary>
        /// <returns>Stringa che rappresenta la data-ora nel formato specificato.</returns>
        public static string GetTimeStamp()
        {
            
            return  "UTC: " + System.DateTime.Now.ToUniversalTime().ToString(TimeStampFormat);
        }

        

        #endregion PublicMethod
    }
}

