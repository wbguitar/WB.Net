using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WB.IIIParty.Commons.Globalization;

namespace WB.IIIParty.Commons.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class KeyboardChangeLanguage : UserControl
    {
        private string selectLanguage = "en-US";
        Dictionary<string, string> lenguage;
        ILanguageManager languageManager;

        /// <summary>
        /// Keyboard change language
        /// </summary>
        public KeyboardChangeLanguage()
        {
            InitializeComponent();

            try
            {
                
                //this.toolStripMenuItemLanguage.Text = "EN";
                //LanguageManager.Instance.Load("language.xml");

                this.Disposed += new EventHandler(KeyboardChangeLanguage_Disposed);

                this.Inizialize(new Dictionary<string, string>());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void KeyboardChangeLanguage_Disposed(object sender, EventArgs e)
        {
            this.toolStripMenuItemLanguage.Dispose();

            this.timerLanguage.Dispose();
        }

        void Instance_CurrentLanguageChanged()
        {
            this.UpdateLanguage();
        }

        /// <summary>
        /// 
        /// </summary>
        public ILanguageManager LanguageManager
        {
            set
            {
                languageManager = value;

                languageManager.CurrentLanguageChanged += new LanguageManager.dCurrentLanguageChanged(Instance_CurrentLanguageChanged);

                this.UpdateLanguage();
            }
        }

        /// <summary>
        /// Inizializza il controllo.
        /// </summary>
        /// <param name="_languagesConfigured">Imposta le lingue che da visualizzare, se null o vuota quelle di default, se non presenti tra quelle installate non viene visualizzata.</param>
        public void Inizialize(Dictionary<string, string> _languagesConfigured)
        {
            try
            {
                this.lenguage = new Dictionary<string, string>();
                InputLanguageCollection inpLgColl = InputLanguage.InstalledInputLanguages;
                foreach (InputLanguage item in inpLgColl)
                {
                    lenguage.Add(item.Culture.ToString(), item.Culture.ToString());
                }

                Dictionary<string, string> lenguageView = new Dictionary<string, string>();
                if ((_languagesConfigured != null) && (_languagesConfigured.Count > 0))
                {
                    foreach (KeyValuePair<string, string> item in _languagesConfigured)
                    {
                        if (this.lenguage.ContainsKey(item.Key))
                            lenguageView.Add(item.Key, item.Value);
                    }
                    this.lenguage = lenguageView;
                }

                CreateMenu();

                //Imposta come default la prima lingua della lista.
                //InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo(this.selectLanguage));
                //this.toolStripMenuItemLanguage.Text = InputLanguage.CurrentInputLanguage.Culture.Name.ToString().Substring(0, 2).ToUpper();

                UpdateLanguage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        void KeyboardChangeLanguage_Click(object sender, EventArgs e)
        {
            try
            {
                if(languageManager!=null)
                    this.toolStripMenuItemLanguage.Text = languageManager.Translate(((ToolStripItem)sender).Tag.ToString().Substring(0, 2).ToUpper());

                this.selectLanguage = ((ToolStripItem)sender).Text.ToString();

                InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo(((ToolStripItem)sender).Tag.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// Return the current select language.
        /// </summary>
        public string CurrentSelectLanguage
        {
            get { return this.selectLanguage; }
        }
        
        #region Private Method
        /// <summary>
        /// 
        /// </summary>
        delegate void dUpdateLanguage();
        private void UpdateLanguage()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new dUpdateLanguage(UpdateLanguage));
                return;
            }

            CreateMenu();

            if (languageManager != null)
            {
                if (languageManager.RightToLeft)
                {
                    this.toolStripMenuItemLanguage.RightToLeft = RightToLeft.Yes;
                    return;
                }
            }
            this.toolStripMenuItemLanguage.RightToLeft = RightToLeft.No;            

        }

        private void CreateMenu()
        {
            this.toolStripMenuItemLanguage.DropDownItems.Clear();
            int i = 0;
            ToolStripItem[] arrayToolStripItem = new ToolStripItem[this.lenguage.Count];

            string text2 = InputLanguage.CurrentInputLanguage.Culture.Name.ToString().Substring(0, 2).ToUpper();
            if (languageManager != null) text2 = languageManager.Translate(InputLanguage.CurrentInputLanguage.Culture.Name.ToString().Substring(0, 2).ToUpper());

            foreach (KeyValuePair<string, string> item in this.lenguage)
            {
                string text = item.Value.ToString();
                if (languageManager != null) text = languageManager.Translate(item.Value.ToString());                

                arrayToolStripItem[i] = new ToolStripMenuItem(item.Key);
                arrayToolStripItem[i].AutoSize = false;
                arrayToolStripItem[i].BackColor = Color.WhiteSmoke;
                arrayToolStripItem[i].ForeColor = Color.Black;
                arrayToolStripItem[i].Name = "toolStripMenuItem_" + i.ToString();
                arrayToolStripItem[i].DisplayStyle = ToolStripItemDisplayStyle.Text;
                arrayToolStripItem[i].Size = new System.Drawing.Size(150, 20);
                arrayToolStripItem[i].Text = text;
                arrayToolStripItem[i].Click += new EventHandler(KeyboardChangeLanguage_Click);
                arrayToolStripItem[i].Tag = item.Key;
                i++;
            }
            this.toolStripMenuItemLanguage.DropDownItems.AddRange(arrayToolStripItem);
            this.toolStripMenuItemLanguage.Text = text2;
            //  ToolStrip:
        }

        #endregion

        private void timerLanguage_Tick(object sender, EventArgs e)
        {
            this.toolStripMenuItemLanguage.Text = InputLanguage.CurrentInputLanguage.Culture.Name.ToString().Substring(0, 2).ToUpper();
        }
    }
}
