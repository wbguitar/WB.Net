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

namespace WB.IIIParty.Commons.Logger
{
    #region Enum

    /// <summary>
    /// Enumera le formattazioni di stampa
    /// </summary>
    public enum PrintTypeByteArray { 
        /// <summary>
        /// ASCII
        /// </summary>
        Ascii, 
        /// <summary>
        /// Esadecimale
        /// </summary>
        Hexadecimal, 
        /// <summary>
        /// Decimale senza segno
        /// </summary>
        Decimal };

    #endregion

    /// <summary>
    /// Enumera le direzioni dei dati in uno stream
    /// </summary>
    public enum TraceDirections
    {
        /// <summary>
        /// Dati in ingresso
        /// </summary>
        Input = 0,
        /// <summary>
        /// Dati in uscita
        /// </summary>
        Output = 1,        

    }

    /// <summary>
    /// Interfaccia generica di Log di uno stream di dati
    /// </summary>
    public interface ITraceLog
    {

        /// <summary>
        /// Inserisce un log di un flusso di dati
        /// </summary>
        /// <param name="_currentDevice">Dispositivo locale</param>
        /// <param name="_remoteDevice">Dispositivo remoto</param>
        /// <param name="_data">Dati da storicizzare</param>
        /// <param name="_direction">Direzione dei dati rispetto al Dispositivo locale</param>
        /// <param name="_description">Descrizione aggiuntiva</param>
        /// <param name="_printTypeByteArray">Formattazione di stampa</param>
        void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction,
            string _description, PrintTypeByteArray _printTypeByteArray);
        
    }
}
