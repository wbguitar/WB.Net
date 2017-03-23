// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2010
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-06-14 11:23:59 +0200 (ven, 14 giu 2013) $
//Versione: $Rev: 154 $
// ------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using WB.IIIParty.Commons.Logger;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace WB.IIIParty.Commons.Globalization
{
    /// <summary>
    /// Consente la gestione di testi multilingua
    /// </summary>
    public interface ILanguageManager
    {
     
        
        /// <summary>
        /// Notifica della variazione della lingua corrente
        /// </summary>
        event LanguageManager.dCurrentLanguageChanged CurrentLanguageChanged;
        /// <summary>
        /// Notifica della variazione della lingua corrente
        /// </summary>
        event LanguageManager.dBeforeCurrentLanguageChanged BeforeCurrentLanguageChanged;
        /// <summary>
        /// Notifica della variazione della lingua corrente
        /// </summary>
        event LanguageManager.dAfterCurrentLanguageChanged AfterCurrentLanguageChanged;

        /// <summary>
        /// Carica le traduzioni da un file di configurazione
        /// </summary>
        /// <param name="fileName"></param>
        void Load(string fileName);
        /// <summary>
        /// Ritorna il testo tradotto nella lingua corrente
        /// </summary>
        /// <param name="text">Testo da tradurre</param>
        /// <returns>testo tradotto</returns>
        string Translate(string text);

        /// <summary>
        /// Ritorna il testo tradotto nella lingua corrente
        /// </summary>
        /// <param name="text">Testo da tradurre</param>
        /// <returns>testo tradotto</returns>
        LanguageManager.TextTranslated TranslateToText(string text);

        /// <summary>
        /// Ritorna tutte le informazioni di traduzione del testo di input
        /// </summary>
        /// <param name="text">Testo di ricerca</param>
        /// <returns>Traduzione completa</returns>
        LanguageManager.Text GetText(string text);

        /// <summary>
        /// Ritorna il testo tradotto nella lingua specificata
        /// </summary>
        /// <param name="text">Testo da tradurre</param>
        /// <param name="language">Lingua specifica</param>
        /// <returns>testo tradotto</returns>
        string Translate(string text,string language);
 
        /// <summary>
        /// Aggiunge un testo con la relativa traduzione
        /// </summary>
        /// <param name="text"></param>
        void Add(LanguageManager.Text text);

        /// <summary>
        /// Aggiorna la lingua corrente su chiave di registro
        /// </summary>
        /// <param name="?"></param>
        void SetLanguage(string language);

        /// <summary>
        /// Imposta o ritorna la lingua corrente
        /// </summary>
        string CurrentLanguage
        {
            get;            
            set;            
        }
       

        /// <summary>
        /// Ritorna l'orientamento della lingua
        /// </summary>
        bool RightToLeft
        {
            get;
            
        }

        
    }
}
