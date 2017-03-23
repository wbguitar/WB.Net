// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WB.IIIParty.Commons.IO
{
    /// <summary>
    /// Implementa metodi di utility con le directory
    /// </summary>
    public class Directory
    {

        #region Static Members

        /// <summary>
        /// Imposta il path passato come working directory corrente
        /// </summary>
        /// <returns>Path</returns>
        public static void SetCurrentDirectory(string path)
        {
            System.IO.Directory.SetCurrentDirectory(path);
        }

        /// <summary>
        /// Imposta il path dell'assembly di esecuzione come working directory corrente
        /// </summary>
        /// <returns></returns>
        public static void SetCurrentDirectory()
        {
            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Ritorna il path dell'assembly di esecuzione
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        #endregion

    }
}
