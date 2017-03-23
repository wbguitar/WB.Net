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
using WB.IIIParty.Commons.Interfaces;

namespace WB.IIIParty.Commons.Windows.Forms
{
    public partial class FormKeepAlive : UserControl , IEnabler
    {

        int counter = 1;

        /// <summary>
        /// 
        /// </summary>
        public FormKeepAlive()
        {
            InitializeComponent();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (counter)
            {
                case 1:
                    {
                        panel1.BackColor = System.Drawing.Color.Blue;
                        panel2.BackColor = System.Drawing.Color.Green;
                        panel3.BackColor = System.Drawing.Color.Red;
                        panel4.BackColor = System.Drawing.Color.Black;
                        break;
                    }
                case 2:
                    {
                        panel1.BackColor = System.Drawing.Color.Black;
                        panel2.BackColor = System.Drawing.Color.Blue;
                        panel3.BackColor = System.Drawing.Color.Green;
                        panel4.BackColor = System.Drawing.Color.Red;
                        break;
                    }
                case 3:
                    {
                        panel1.BackColor = System.Drawing.Color.Red;
                        panel2.BackColor = System.Drawing.Color.Black;
                        panel3.BackColor = System.Drawing.Color.Blue;
                        panel4.BackColor = System.Drawing.Color.Green;
                        break;
                    }
                case 4:
                    {
                        panel1.BackColor = System.Drawing.Color.Green;
                        panel2.BackColor = System.Drawing.Color.Red;
                        panel3.BackColor = System.Drawing.Color.Black;
                        panel4.BackColor = System.Drawing.Color.Blue;
                        break;
                    }
            }

            if (counter == 4) counter = 1;
            else counter++;
        }

        #region IEnabler Members

        /// <summary>
        /// 
        /// </summary>
        public void Enable()
        {
            timer1.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disable()
        {
            timer1.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnable()
        {
            return timer1.Enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            
        }

        #endregion
    }
}
