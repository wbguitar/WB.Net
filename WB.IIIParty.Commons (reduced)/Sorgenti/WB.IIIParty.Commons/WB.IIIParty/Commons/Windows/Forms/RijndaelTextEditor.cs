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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WB.IIIParty.Commons.Security.Cryptography;

namespace WB.IIIParty.Commons.Windows.Forms
{
    /// <summary>
    /// La classe RijndaelTextEditor implementa un controllo di tipologia text editor 
    /// che consente di caricare e salvare file crittografati tramite l’algoritmo Rijndael, 
    /// implementa inoltre funzionalità di Highlights dei linguaggi di programmazione più comuni.
    /// </summary>
    public partial class RijndaelTextEditor : ICSharpCode.TextEditor.TextEditorControl
    {
        #region Enum

        /// <summary>
        /// Elenca i linguaggi di programmazione supportati dal controllo
        /// </summary>
        public enum RijndaelTextEditorHighlights 
        { 
            /// <summary>
            /// Nessun HighLights
            /// </summary>
            None,
            /// <summary>
            /// HighLights linguaggio di programmazione C++ .NET
            /// </summary>
            Cpp,
            /// <summary>
            /// HighLights linguaggio di programmazione Html
            /// </summary>
            Html,
            /// <summary>
            /// HighLights linguaggio di programmazione C# .NET
            /// </summary>
            CSharp,
            /// <summary>
            /// HighLights linguaggio di programmazione Java
            /// </summary>
            Java,
            /// <summary>
            /// HighLights linguaggio di programmazione JavaScript
            /// </summary>
            JavaScript,
            /// <summary>
            /// HighLights linguaggio di programmazione Php
            /// </summary>
            Php,
            /// <summary>
            /// HighLights linguaggio di programmazione VisualBasic .NET
            /// </summary>
            VbNet,
            /// <summary>
            /// HighLights linguaggio di programmazione Xml
            /// </summary>
            Xml }

        #endregion

        #region Constructor

        /// <summary>
        /// Implementa il costruttore della classe RijndaelTextEditor
        /// </summary>
        public RijndaelTextEditor():base()
        {

        }

        #endregion

        #region Public Members

        /// <summary>
        /// Imposta la configurazione Highlights
        /// </summary>
        /// <param name="hl">Linguaggio di programmazione</param>
        public void SetHighlighting(RijndaelTextEditorHighlights hl)
        {
            this.SetHighlighting(hl.ToString());
            switch (hl)
            {
                case RijndaelTextEditorHighlights.Cpp:
                    {
                        this.SetHighlighting("C++.NET");
                        break;
                    }
                case RijndaelTextEditorHighlights.Html:
                    {
                        this.SetHighlighting("HTML");
                        break;
                    }
                case RijndaelTextEditorHighlights.Java:
                    {
                        this.SetHighlighting("Java");
                        break;
                    }
                case RijndaelTextEditorHighlights.CSharp:
                    {
                        this.SetHighlighting("C#");
                        break;
                    }
                case RijndaelTextEditorHighlights.JavaScript:
                    {
                        this.SetHighlighting("JavaScript");
                        break;
                    }
                case RijndaelTextEditorHighlights.Php:
                    {
                        this.SetHighlighting("PHP");
                        break;
                    }
                case RijndaelTextEditorHighlights.VbNet:
                    {
                        this.SetHighlighting("VBNET");
                        break;
                    }
                case RijndaelTextEditorHighlights.Xml:
                    {
                        this.SetHighlighting("XML");
                        break;
                    }
                case RijndaelTextEditorHighlights.None:
                    {
                        this.SetHighlighting("None");
                        break;
                    }
            }
        }

        /// <summary>
        /// Visualizza un file crittografato sul controllo 
        /// </summary>
        /// <param name="path">File sorgente</param>
        /// <param name="key">Chiave di decodifica</param>
        public void LoadFromFile(string path,string key)
        {
            this.Text = RijndaelStringCryptography.DecodeFromFile(path, key);
            this.Refresh();
        }

        /// <summary>
        /// Visualizza un file crittografato sul controllo con una chiave di codifica definita con algoritmo Rijndael
        /// </summary>
        /// <param name="path">File sorgente</param>
        public void LoadFromFile(string path)
        {
            this.Text = RijndaelStringCryptography.DecodeFromFile(path);
            this.Refresh();
        }

        /// <summary>
        /// Salva su un file crittografato il testo visualizzato sul controllo 
        /// </summary>
        /// <param name="path">File sorgente</param>
        /// <param name="key">Chiave di codifica</param>
        /// <param name="overwrite">Sovrascive il file se esiste</param>
        public void SaveToFile(string path, string key,bool overwrite)
        {
            RijndaelStringCryptography.EncodeToFile(this.Text, path, overwrite,key);
            this.Refresh();
        }

        /// <summary>
        /// Salva su un file crittografato il testo visualizzato sul controllo con una chiave di codifica definita con algoritmo Rijndael
        /// </summary>
        /// <param name="path">File di destinazione</param>
        /// <param name="overwrite">Sovrascive il file se esiste</param>
        public void SaveToFile(string path, bool overwrite)
        {            
            RijndaelStringCryptography.EncodeToFile(this.Text,path, overwrite);
            this.Refresh();
        }

        #endregion
    }
}
