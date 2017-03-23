// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2010
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-07-02 13:59:31 +0200 (mar, 02 lug 2013) $
//Versione: $Rev: 166 $
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
    public class LanguageManager : ILanguageManager, IDisposable
    {

        #region Properties

        public bool EnableLog { get; set; }

        #endregion

        #region Constants

        private const string DefaultLoggerName = "LanguageManager";
        private const string PartialLoggerName = "WB.IIIParty.Commons.Globalization";
        //private const string FileName = "LanguageManager.txt";
        private const int FileSize = 2097152; //2 MB
        private string PathRegistry = @"Software\WB.IIIParty\LanguageManager";
        private string KeyName = @"CurrentLanguage";
        private string KeyboardKeyName = @"CurrentKeyboardLanguage";

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultLanguage = "default";
        string languageManagerName;
        #endregion

        #region Internal Class
        /// <summary>
        /// 
        /// </summary>
        public class TextTranslated
        {
            /// <summary>
            /// Identificativo lingua
            /// </summary>
            public string language;
            /// <summary>
            /// Testo tradotto
            /// </summary>
            public string text;
            /// <summary>
            /// Testo tradotto in versione contratta
            /// </summary>
            public string abbreviatedtext;
        }
        /// <summary>
        /// 
        /// </summary>
        public class TextDefault
        {
            /// <summary>
            /// Testo identifier
            /// </summary>
            public string id;
            /// <summary>
            /// Testo default
            /// </summary>
            public string text;
            /// <summary>
            /// Testo default in versione contratta
            /// </summary>
            public string abbreviatedtext;
        }
        /// <summary>
        /// 
        /// </summary>
        public class Text
        {
            /// <summary>
            /// 
            /// </summary>
            public TextDefault Default =new TextDefault();
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, TextTranslated> Languages = new Dictionary<string, TextTranslated>();
        }
        #endregion

        #region Variables
        Dictionary<string, Text> languageDictionary = new Dictionary<string, Text>();

        static LanguageManager languageManager;
        string currentLanguage;
        IMessageLog log;

        /// <summary>
        /// Delega la notifica della variazione della lingua corrente
        /// </summary>
        public delegate void dCurrentLanguageChanged();
        /// <summary>
        /// Notifica della variazione della lingua corrente
        /// </summary>
        public event dCurrentLanguageChanged CurrentLanguageChanged;
        /// <summary>
        /// Delega la notifica della variazione della lingua corrente
        /// </summary>
        public delegate void dBeforeCurrentLanguageChanged();
        /// <summary>
        /// Notifica della variazione della lingua corrente
        /// </summary>
        public event dBeforeCurrentLanguageChanged BeforeCurrentLanguageChanged;
        /// <summary>
        /// Delega la notifica della variazione della lingua corrente
        /// </summary>
        public delegate void dAfterCurrentLanguageChanged();
        /// <summary>
        /// Notifica della variazione della lingua corrente
        /// </summary>
        public event dAfterCurrentLanguageChanged AfterCurrentLanguageChanged;
        //Notifica la variazione della chiave di registro con la lingua corrente
        RegistryKeyChanged registryKeyChanged;

        RegistryKeyChanged.dKEyValues degistryKeyChangedDelegate;

        private static object synchObj = new object();
        #endregion

        #region Singleton Instance
        /// <summary>
        /// Istanza di LanguageManager
        /// </summary>
        public static LanguageManager Instance
        {
            get
            {
                lock (synchObj)
                {
                    if (languageManager == null)
                        languageManager = new LanguageManager();
                    return languageManager;
                }
            }
        }
        #endregion

        #region Constructor

        string GetLoggerName()
        {
            return PartialLoggerName + "." + languageManagerName;
        }

        string GetFileLoggerName()
        {
            return languageManagerName + ".txt";
        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageManager():this(DefaultLoggerName)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageManager(string _languageManagerName)
        {
            languageManagerName = _languageManagerName;

            EnableLog = true;

            ReadCurrentLanguageFromRegistry();

            ReadCurrentKeyboardLanguageFromRegistry();

            registryKeyChanged = new RegistryKeyChanged(PathRegistry);
            degistryKeyChangedDelegate = new RegistryKeyChanged.dKEyValues(OnRegistryChanged);
            registryKeyChanged.AddNotify(degistryKeyChangedDelegate);

            CircularFileMessageLoggerConfig logger = new CircularFileMessageLoggerConfig(GetLoggerName(),
                    LogLevels.Debug, GetFileLoggerName(), FileSize);
            try
            {
                LoggerManager.CreateLogger(logger);
            }
            catch (Logger.LoggerManager.DuplicateNameException) { }

            log = LoggerManager.GetLogger<IMessageLog>(GetLoggerName());

        }
        #endregion

        #region Private Methods

        void WriteCurrentLanguageToRegistry(string language)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(PathRegistry, true);            
            key.SetValue(KeyName, language);
            key.Flush();
            key.Close();
            currentLanguage = language;
        }

        void WriteCurrentKeyboardLanguageToRegistry(string language)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(PathRegistry, true);
            key.SetValue(KeyboardKeyName, language);
            key.Flush();
            key.Close();
        }

        void OnRegistryChanged(Hashtable values)
        {
            if(values.ContainsKey(KeyName))
            {
                string language = values[KeyName].ToString();
                this.CurrentLanguage = language;
            }
            if (values.ContainsKey(KeyboardKeyName))
            {
                string language = values[KeyboardKeyName].ToString();
                this.CurrentKeyboardLanguage = language;
            }
        }

        void ReadCurrentLanguageFromRegistry()
        {
            RegistryKey key = Registry.LocalMachine.CreateSubKey(PathRegistry);
            currentLanguage = (string)key.GetValue(KeyName);
            if (currentLanguage == null)
                WriteCurrentLanguageToRegistry(DefaultLanguage);
            key.Close();            
        }

        void ReadCurrentKeyboardLanguageFromRegistry()
        {
            RegistryKey key = Registry.LocalMachine.CreateSubKey(PathRegistry);
            currentLanguage = (string)key.GetValue(KeyName);
            if (currentLanguage == null)
                WriteCurrentLanguageToRegistry(DefaultLanguage);
            key.Close();
        }

        private void AddTranslation(Text text)
        {
            if (!languageDictionary.ContainsKey(text.Default.text))
                this.languageDictionary.Add(text.Default.text, text);
            else
            {
                LanguageManager.Text t = this.languageDictionary[text.Default.text];
                foreach (LanguageManager.TextTranslated translated in text.Languages.Values)
                {
                    if(!t.Languages.ContainsKey(translated.language))
                        t.Languages.Add(translated.language,translated);
                }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Carica il file xml di configurazione specificato
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            languageDictionary.Clear();

            XmlReaderSettings setting = new XmlReaderSettings();
            setting.ValidationType = ValidationType.None;
            
            StreamReader stream =new StreamReader( fileName,System.Text.UTF8Encoding.UTF8);
            XmlReader reader = XmlReader.Create(stream);
            
            List<Text> textList = new List<Text>();
            Text singleText = new Text();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "text":
                            {
                                singleText = new Text();
                                textList.Add(singleText);
                                break;
                            }
                        case DefaultLanguage:
                            {
                                if (reader.Read())
                                {
                                    singleText.Default.text = reader.Value;
                                }
                                break;
                            }
                        case "translation":
                            {
                                TextTranslated translated =new TextTranslated();
                                translated.language = reader.GetAttribute("language");
                                if (reader.Read())
                                {
                                    translated.text = reader.Value;
                                }
                                if (!singleText.Languages.ContainsKey(translated.language))
                                singleText.Languages.Add(translated.language,translated);
                                break;
                            }


                    }
                }

            }

            foreach (Text text in textList)
            {
                AddTranslation(text);                
            }

        }

        /// <summary>
        /// Ritorna il testo tradotto nella lingua corrente
        /// </summary>
        /// <param name="text">Testo da tradurre</param>
        /// <returns>testo tradotto</returns>
        public string Translate(string text)
        {
            return TranslateToText(text).text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public LanguageManager.TextTranslated TranslateToText(string text)
        {
            LanguageManager.TextTranslated t_text = new TextTranslated();
            t_text.text = text;
            t_text.abbreviatedtext = text;
            
            if (text == null)
            {
                return t_text;
            }
            if (!languageDictionary.ContainsKey(text))
            {
                if (EnableLog)
                    log.Log(LogLevels.Error, "Text not found: " + text);

                return t_text;
            }
            if (currentLanguage == DefaultLanguage)
            {
                return t_text;
            }
            if (!languageDictionary[text].Languages.ContainsKey(currentLanguage))
            {
                if (EnableLog)
                    log.Log(LogLevels.Error, "Language not found: " + currentLanguage + " for Text: " + text);

                return t_text;
            }

            return languageDictionary[text].Languages[currentLanguage];            
        }

        /// <summary>
        /// Ritorna il testo tradotto nella lingua specificata
        /// </summary>
        /// <param name="text">Testo da tradurre</param>
        /// <param name="language">Lingua specifica</param>
        /// <returns>testo tradotto</returns>
        public string Translate(string text,string language)
        {
            if (text == null)
            {
                return text;
            }
            if (!languageDictionary.ContainsKey(text))
            {
                if (EnableLog)
                    log.Log(LogLevels.Error, "Text not found: " + text);

                return text;
            }
            if ((language == DefaultLanguage)||(language==string.Empty))
            {
                return text;
            }
            if (!languageDictionary[text].Languages.ContainsKey(language))
            {
                if (EnableLog)
                    log.Log(LogLevels.Error, "Language not found: " + language + " for Text: " + text);

                return text;
            }

            string translateText = languageDictionary[text].Languages[language].text;
            if (translateText != string.Empty)
                return translateText;
            return text;
        }

        /// <summary>
        /// Aggiunge un testo con la relativa traduzione
        /// </summary>
        /// <param name="text"></param>
        public void Add(Text text)
        {
            AddTranslation(text);
        }

        /// <summary>
        /// Imposta o ritorna la lingua corrente
        /// </summary>
        public string CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                if (!currentLanguage.Equals(value))
                {
                    if (BeforeCurrentLanguageChanged != null) BeforeCurrentLanguageChanged();

                    WriteCurrentLanguageToRegistry(value);

                    if (CurrentLanguageChanged != null)
                    {
                        CurrentLanguageChanged();

                        Console.WriteLine("Lo ricevono in: " + CurrentLanguageChanged.GetInvocationList().Length);
                    }

                    if (AfterCurrentLanguageChanged != null) AfterCurrentLanguageChanged();
                }
            }
        }

        /// <summary>
        /// Ricarica la lingua corrente alla tastiera
        /// </summary>
        public void ReloadKeyboardLanguage()
        {
            this.CurrentKeyboardLanguage = this.CurrentKeyboardLanguage;
        }

        /// <summary>
        /// Imposta o ritorna la tastiera corrente
        /// </summary>
        public string CurrentKeyboardLanguage
        {
            get
            {
                return InputLanguage.CurrentInputLanguage.Culture.Name;
            }
            set
            {
                try
                {
                    InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo(value));
                    
                    WriteCurrentKeyboardLanguageToRegistry(InputLanguage.CurrentInputLanguage.Culture.Name);
                }
                catch { }
            }
        }

        /// <summary>
        /// Ritorna l'orientamento della lingua
        /// </summary>
        public bool RightToLeft
        {
            get
            {
                try
                {
                    return new System.Globalization.CultureInfo(currentLanguage).TextInfo.IsRightToLeft;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.registryKeyChanged.RemoveNotify(this.degistryKeyChangedDelegate);
            this.registryKeyChanged.Dispose();

            LoggerManager.DestroyLogger(GetLoggerName());            
        }

        #endregion


        #region ILanguageManager Members


        public LanguageManager.TextTranslated Translate(string text, bool dummyparam)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ILanguageManager Members

        /// <summary>
        /// Aggiorna la lingua corrente su chiave di registro
        /// </summary>
        /// <param name="language"></param>
        public void SetLanguage(string language)
        {
            WriteCurrentLanguageToRegistry(language);
        }

        #endregion

        #region ILanguageManager Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public LanguageManager.Text GetText(string text)
        {
            if (languageDictionary.ContainsKey(text))
            {
                return languageDictionary[text];
            }
            return null;
        }

        #endregion
    }
}
