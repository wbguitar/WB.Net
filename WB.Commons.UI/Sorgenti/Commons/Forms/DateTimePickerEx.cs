using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WB.Commons.Forms
{
    public class DateTimePickerEx : System.Windows.Forms.DateTimePicker
    {
        private System.Windows.Forms.TextBox txtDateTime;
        private System.ComponentModel.IContainer components;

        private bool SetDate;
        private System.Windows.Forms.ErrorProvider ErrorMessage;
        private System.Windows.Forms.ToolTip Tooltip;

        private const int BTNWIDTH = 16;

        private CultureInfo culture = CultureInfo.CurrentCulture;

        public CultureInfo Culture
        {
            get { return culture; }
            set
            {
                if (!culture.Equals(value))
                {
                    culture = value;
                    CultureChanged();
                }
            }
        }

        public event Action CultureChanged = ()=> { };

        public enum dtpCustomExtensions
        {
            dtpLong = 0,
            dtpShort = 1,
            dtpTime = 2,
            dtpShortDateShortTimeAMPM = 3,
            dtpShortDateLongTimeAMPM = 4,
            dtpShortDateShortTime24Hour = 5,
            dtpShortDateLongTime24Hour = 6,
            dtpLongDateShortTimeAMPM = 7,
            dtpLongDateLongTimeAMPM = 8,
            dtpLongDateShortTime24Hour = 9,
            dtpLongDateLongTime24Hour = 10,
            dtpSortableDateAndTimeLocalTime = 11,
            dtpUTFLocalDateAndShortTimeAMPM = 12,
            dtpUTFLocalDateAndLongTimeAMPM = 13,
            dtpUTFLocalDateAndShortTime24Hour = 14,
            dtpUTFLocalDateAndLongTime24Hour = 15,
            dtpShortTimeAMPM = 16,
            dtpShortTime24Hour = 17,
            dtpLongTime24Hour = 18,
            dtpYearAndMonthName = 19,
            dtpMonthNameAndDay = 20,
            dtpYear4Digit = 21,
            dtpMonthFullName = 22,
            dtpMonthShortName = 23,
            dtpDayFullName = 24,
            dtpDayShortName = 25,
            dtpShortDateAMPM = 26,
            dtpShortDateMorningAfternoon = 27,
            dtpCustom = 28
        }

        private string mvarLinkedTo;
        private bool bDroppedDown;
        private int ButtonWidth = BTNWIDTH;
        private bool mvarShowButtons = true;
        private dtpCustomExtensions mvarFormatEx;
        private string mvarCustomFormatMessage;
        private int CheckWidth = 0;

        private DateTimePickerEx LinkTo;
        private System.Collections.ArrayList LinkToArray = new System.Collections.ArrayList();
        private System.Collections.ArrayList LinkedArray = new System.Collections.ArrayList();

        #region Constructor and destructor

        public DateTimePickerEx()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            //Initialise bas.Format to Custom, we only need Custom Format
            base.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            DateTimePicker_Resize(this, null);

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Constructor and destructor

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtDateTime = new System.Windows.Forms.TextBox();
            this.ErrorMessage = new System.Windows.Forms.ErrorProvider();
            this.Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // txtDateTime
            // 
            this.txtDateTime.Location = new System.Drawing.Point(20, 49);
            this.txtDateTime.MaxLength = 50;
            this.txtDateTime.Name = "txtDateTime";
            this.txtDateTime.TabIndex = 0;
            this.txtDateTime.Text = "";
            this.txtDateTime.BackColorChanged += new System.EventHandler(this.txtDateTime_BackColorChanged);
            this.txtDateTime.Leave += new System.EventHandler(this.txtDateTime_Leave);
            this.txtDateTime.Enter += new System.EventHandler(this.txtDateTime_Enter);
            // 
            // ErrorMessage
            // 
            this.ErrorMessage.DataMember = null;
            // 
            // DateTimePickerEx
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.txtDateTime });
            this.Value = new System.DateTime(1753, 1, 1, 15, 8, 40, 119);
            this.DropDown += new System.EventHandler(this.DateTimePicker_DropDown);
            this.FontChanged += new System.EventHandler(this.DateTimePicker_FontChanged);
            this.Resize += new System.EventHandler(this.DateTimePicker_Resize);
            this.Enter += new System.EventHandler(this.DateTimePicker_Enter);
            this.CloseUp += new System.EventHandler(this.DateTimePicker_CloseUp);
            this.ForeColorChanged += new System.EventHandler(this.DateTimePicker_ForeColorChanged);
            this.BackColorChanged += new System.EventHandler(this.DateTimePicker_BackColorChanged);
            this.ValueChanged += new System.EventHandler(this.FormatOrValueChanged);
            this.FormatChanged += new System.EventHandler(this.FormatOrValueChanged);
            this.ResumeLayout(false);

        }
        #endregion

        #region overriden and additional properties

        //OverRide Formst and hide it by setting Browsable false, make it read only
        //so it can't be written to, it will always be Custom anyway
        [Browsable(false)]
        public new System.Windows.Forms.DateTimePickerFormat Format
        {
            get
            {
                return base.Format;
            }
            //set
            //{
            //	base.Format = value;
            //}
        }

        //FormatEx, extends the formatting options by allowing additional selections
        //Replaces Format
        [Browsable(true), Category("Appearance"), Description("Format Extensions replaces Format gets sets display Formats")]
        public dtpCustomExtensions FormatEx
        {
            get
            {
                return mvarFormatEx;
            }
            set
            {
                mvarFormatEx = value;
                InitialiseCustomMessage();
            }
        }

        //New Property, allows hiding of DropDown Button and Updown Button
        [Browsable(true), Category("Appearance"), Description("Hides DropDown and Spin Buttons, Allows keyed entry only.")]
        public bool ShowButtons
        {
            get
            {
                return mvarShowButtons;
            }
            set
            {
                //Do not allow Set Show Buttons when ReadOnly is true
                //all Buttons and Chexkbox are hidden when Control is Read Only
                if (!this.ReadOnly)
                {
                    mvarShowButtons = value;
                    if (mvarShowButtons)
                    {
                        ButtonWidth = BTNWIDTH;
                    }
                    else
                    {
                        ButtonWidth = 0;
                    }
                    DateTimePicker_Resize(this, null);
                }
            }
        }

        //Overrides base.ShowCheckBox
        [Browsable(true), Category("Appearance"), Description("Hides DropDown and Spin Buttons, Allows keyed entry only.")]
        public new bool ShowCheckBox
        {
            get
            {
                return base.ShowCheckBox;
            }
            set
            {
                //Do not allow set ShowCheckBox when ReadOnly is True
                //all Buttons and Chexkbox are hidden when Control is Read Only
                if (!this.ReadOnly)
                {
                    base.ShowCheckBox = value;
                    if (base.ShowCheckBox)
                    {
                        CheckWidth = BTNWIDTH;
                    }
                    else
                    {
                        CheckWidth = 0;
                    }
                    DateTimePicker_Resize(this, null);
                }
            }
        }

        //overrie Text, we want to set Get Textbox Text
        [Browsable(true), Category("Behavior"), Description("Date and Time displayed")]
        public new string Text
        {
            get
            {
                return txtDateTime.Text;
            }
            set
            {
                txtDateTime.Text = value;
                //Don't bother Formatting the Textbox if it's value is NullString
                //It will cause problems if you do
                if (value != "")
                {
                    FormatTextBox();
                }
            }
        }

        //Override bas.ShowUpDown
        [Browsable(true), Category("Appearance"), Description("Uses Updown control to select dates instead of Dropdown control")]
        public new bool ShowUpDown
        {
            get
            {
                return base.ShowUpDown;
            }
            set
            {
                //Do not allow set ShowUpDown when ReadOnly is True
                //all Buttons and Checkbox are hidden when Control is Read Only
                if (!this.ReadOnly)
                {
                    base.ShowUpDown = value;
                    txtDateTime.Text = "";
                }
            }
        }

        //Override Textbox back Colour so we can add it to the Appearance List
        //and use it to set the BG colour
        [Browsable(true), Category("Appearance"), Description("The Backround Colour user to display Text and Graphics in this Control")]
        public new System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        //New Property Read Only makes it possible to set Textbox to read only
        [Browsable(true), Category("Behavior"), Description("Used to set whether the control can be edited")]
        public bool ReadOnly
        {
            get
            {
                return txtDateTime.ReadOnly;
            }
            set
            {
                //If ReadOnly is true make sure ShowCheckBox, ShowUpDown and ShowButtons 
                //are false.
                //all Buttons and Checkbox are hidden when Control is Read Only
                //Be aware of the order these properties are set
                if (value)
                {
                    this.ShowCheckBox = false;
                    this.ShowUpDown = false;
                    this.ShowButtons = false;
                    txtDateTime.ReadOnly = value;
                }
                else
                {
                    txtDateTime.ReadOnly = value;
                    this.ShowButtons = true;
                }
            }
        }

        //New Property Makes it possible to link control to another Datetimepicker
        [Browsable(true), Category("Behavior"), Description("Set Get another Date Picker Control that this control receives data from.")]
        public string LinkedTo
        {
            get
            {
                return mvarLinkedTo;
            }
            set
            {
                mvarLinkedTo = value;
                LinkedArray.Clear();
                if (mvarLinkedTo != "" && mvarLinkedTo != null)
                {
                    string[] splitmvarLinkedTo = mvarLinkedTo.Split(",".ToCharArray());
                    for (int i = 0; i < splitmvarLinkedTo.Length; i++)
                    {
                        LinkedArray.Add(splitmvarLinkedTo[i].Trim());
                    }
                }
            }
        }

        #endregion

        #region DateTimePickerEx events

        private void DateTimePicker_Resize(object sender, System.EventArgs e)
        {
            this.txtDateTime.Location = new System.Drawing.Point(-2 + CheckWidth, -2);
            this.txtDateTime.Size = new System.Drawing.Size(this.Width - ButtonWidth - CheckWidth, this.Height);
        }

        private void DateTimePicker_FontChanged(Object sender, System.EventArgs e)
        {
            //Make sure TextBox Font =  Dtp Font
            txtDateTime.Font = this.Font;
        }

        private void DateTimePicker_BackColorChanged(Object sender, System.EventArgs e)
        {
            //Make sure TextBox BackColour =  Dtp Back Colour
            txtDateTime.BackColor = this.BackColor;
        }

        private void txtDateTime_BackColorChanged(Object sender, System.EventArgs e)
        {
            //Make sure DTP BackColour =  TextBox Back Colour
            if (txtDateTime.BackColor != this.BackColor)
            {
                this.BackColor = txtDateTime.BackColor;
            }
        }

        private void DateTimePicker_ForeColorChanged(Object sender, System.EventArgs e)
        {
            //Make sure TextBox Fore Colour =  Dtp Fore Colour
            txtDateTime.ForeColor = this.BackColor;
        }

        private void FormatOrValueChanged(Object sender, System.EventArgs e)
        {
            ErrorMessage.SetError(this, "");

            //if dtp Value changed 
            //Attempt to Format the TextBox String if Text is not NullString
            if (this.Text != "")
            {
                try
                {
                    FormatTextBox();
                }
                catch
                {
                    ErrorMessage.SetError(this, "Invalid Date - " + txtDateTime.Text + ", valid format is " + mvarCustomFormatMessage);
                }
            }
        }

        private void txtDateTime_Enter(Object sender, System.EventArgs e)
        {
            Tooltip.SetToolTip(txtDateTime, mvarCustomFormatMessage);

            if (txtDateTime.Text.Length > 0)
            {
                txtDateTime.SelectionStart = 0;
                txtDateTime.SelectionLength = txtDateTime.Text.Length;
            }

            SetDate = true;
            this.Value = DateTime.Now;
            SetDate = false;
        }

        private void txtDateTime_Leave(Object sender, System.EventArgs e)
        {
            if (!SetDate)
            {
                SetDate = true;

                ErrorMessage.SetError(this, "");

                //Attempt to Format the TextBox String if Text is not NullString
                if (this.Text != "")
                {
                    try
                    {
                        FormatTextBox();
                        //if Link To is Not nullString
                        //Attempt to Link to the Specified LinkTo Controls
                        LinkToArray.Clear();
                        if (mvarLinkedTo != "" && mvarLinkedTo != null)
                        {
                            for (int j = 0; j < LinkedArray.Count; j++)
                            {
                                for (int i = 0; i < this.Parent.Controls.Count; i++)
                                {
                                    if (this.Parent.Controls[i].Name == LinkedArray[j].ToString() && this.Parent.Controls[i] is DateTimePickerEx)
                                    {
                                        LinkTo = (DateTimePickerEx)this.Parent.Controls[i];
                                        LinkToArray.Add(LinkTo);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        ErrorMessage.SetError(this, "Invalid Date - " + txtDateTime.Text + ", valid format is " + mvarCustomFormatMessage);
                    }
                }

                //IF the LinkTo Object has been instantiated it's ok to attempt to set it's Text Value
                for (int i = 0; i < LinkToArray.Count; i++)
                {
                    if (this.LinkToArray[i] != null)
                    {
                        LinkTo = (DateTimePickerEx)LinkToArray[i];
                        LinkTo.Text = this.Text;
                    }
                }

                SetDate = false;
            }
        }

        private void DateTimePicker_Enter(Object sender, System.EventArgs e)
        {
            txtDateTime.Focus();
        }

        private void DateTimePicker_DropDown(Object sender, System.EventArgs e)
        {
            bDroppedDown = true;
        }

        private void DateTimePicker_CloseUp(object sender, System.EventArgs e)
        {
            if (bDroppedDown || this.ShowUpDown)
            {
                if (!SetDate)
                {
                    txtDateTime.Text = this.Value.ToString(Culture);
                    FormatTextBox();
                    bDroppedDown = false;
                    txtDateTime.Focus();
                }
            }
        }

        protected override void OnValueChanged(System.EventArgs eventargs)
        {


            if (bDroppedDown || this.ShowUpDown)
            {
                if (!SetDate)
                {
                    txtDateTime.Text = this.Value.ToString(Culture);
                    FormatTextBox();
                }
            }
        }

        //Set up the message that will diplay in the Tooltip
        //when the mouse is hovered over the control
        private void InitialiseCustomMessage()
        {
            switch (mvarFormatEx)
            {
                case dtpCustomExtensions.dtpCustom:
                    mvarCustomFormatMessage = this.CustomFormat;
                    break;
                case dtpCustomExtensions.dtpLong:
                    mvarCustomFormatMessage = "Long Date (" + DateTime.Now.ToLongDateString() + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShort:
                    mvarCustomFormatMessage = "Short Date (" + DateTime.Now.ToShortDateString() + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpTime:
                    mvarCustomFormatMessage = "Long Time AM/PM (" + DateTime.Now.ToLongTimeString() + ")";
                    this.CustomFormat = "HH:mm:ss yyyy-MM-dd ";
                    break;
                case dtpCustomExtensions.dtpDayFullName:
                    mvarCustomFormatMessage = "Day of the Week Full Name (" + DateTime.Now.ToString("dddd", Culture) + ")";
                    this.CustomFormat = "dd-MM-yyyy HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpDayShortName:
                    mvarCustomFormatMessage = "Day of the Week Short Name (" + DateTime.Now.ToString("ddd", Culture) + ")";
                    this.CustomFormat = "dd-MM-yyyy HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpLongDateLongTime24Hour:
                    mvarCustomFormatMessage = "Long Date Long Time 24 Hour (" + DateTime.Now.ToString("D", Culture) + " " + DateTime.Now.ToString("HH:mm:ss", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpLongDateLongTimeAMPM:
                    mvarCustomFormatMessage = "Long Date Long Time AM/PM (" + DateTime.Now.ToString("D", Culture) + " " + DateTime.Now.ToString("hh:mm:ss tt", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpLongDateShortTime24Hour:
                    mvarCustomFormatMessage = "Long Date Short Time 24 Hour (" + DateTime.Now.ToString("D", Culture) + " " + DateTime.Now.ToString("HH:mm", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpLongDateShortTimeAMPM:
                    mvarCustomFormatMessage = "Long Date Short Time AM/PM (" + DateTime.Now.ToString("D", Culture) + " " + DateTime.Now.ToString("hh:mm tt", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpLongTime24Hour:
                    mvarCustomFormatMessage = "Long Time 24 Hour (" + DateTime.Now.ToString("HH:mm:ss", Culture) + ")";
                    this.CustomFormat = "HH:mm:ss yyyy-MM-dd ";
                    break;
                case dtpCustomExtensions.dtpMonthFullName:
                    mvarCustomFormatMessage = "Month Full Name (" + DateTime.Now.ToString("MMMM", Culture) + ")";
                    this.CustomFormat = "MM-dd-yyyy HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpMonthNameAndDay:
                    mvarCustomFormatMessage = "Month Name and Day (" + DateTime.Now.ToString("M", Culture) + ")";
                    this.CustomFormat = "dd-MM-yyyy HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpMonthShortName:
                    mvarCustomFormatMessage = "Month Short Name (" + DateTime.Now.ToString("MMM", Culture) + ")";
                    this.CustomFormat = "MM-dd-yyyy HH:mm:ss";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortDateLongTime24Hour:
                    mvarCustomFormatMessage = "Short Date Long Time 24 Hour (" + DateTime.Now.ToString("d", Culture) + " " + DateTime.Now.ToString("HH:mm:ss", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortDateLongTimeAMPM:
                    mvarCustomFormatMessage = "Short Date Long Time AM/PM (" + DateTime.Now.ToString("G", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortDateShortTime24Hour:
                    mvarCustomFormatMessage = " Short Date Short Time 24 Hour (" + DateTime.Now.ToString("d", Culture) + " " + DateTime.Now.ToString("HH:mm", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortDateShortTimeAMPM:
                    mvarCustomFormatMessage = " Short Date Short Time AM/PM (" + DateTime.Now.ToString("d", Culture) + " " + DateTime.Now.ToString("hh:mmss tt", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortTime24Hour:
                    mvarCustomFormatMessage = "Short Time 24 Hour (" + DateTime.Now.ToString("HH:mm", Culture) + ")";
                    this.CustomFormat = "HH:mm:ss yyyy-MM-dd ";
                    break;
                case dtpCustomExtensions.dtpShortTimeAMPM:
                    mvarCustomFormatMessage = "Short Time AM/PM (" + DateTime.Now.ToString("hh:mm tt", Culture) + ")";
                    this.CustomFormat = "HH:mm:ss yyyy-MM-dd ";
                    break;
                case dtpCustomExtensions.dtpSortableDateAndTimeLocalTime:
                    mvarCustomFormatMessage = "Sortable Date and Local Time (" + DateTime.Now.ToString("s", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndLongTime24Hour:
                    mvarCustomFormatMessage = "UTF Local Date and Long Time 24 Hour (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndLongTimeAMPM:
                    mvarCustomFormatMessage = "UTF Local Date and Long Time AM/PM (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndShortTime24Hour:
                    mvarCustomFormatMessage = "UTF Local Date and Short Time 24 Hour (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndShortTimeAMPM:
                    mvarCustomFormatMessage = "UTF Local Date and Short Time AM/PM (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm tt", Culture) + ")";
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpYear4Digit:
                    mvarCustomFormatMessage = "4 Digit Year (" + DateTime.Now.ToString("yyyy", Culture);
                    this.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpYearAndMonthName:
                    mvarCustomFormatMessage = "Year and Month Name (" + DateTime.Now.ToString("Y", Culture) + ")";
                    this.CustomFormat = "MM-dd-yyyy HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortDateAMPM:
                    mvarCustomFormatMessage = "Short Date AM/PM (" + DateTime.Now.ToString("d", Culture) + " " + DateTime.Now.ToString("tt", Culture) + ")";
                    this.CustomFormat = "MM-dd-yyyy HH:mm:ss";
                    break;
                case dtpCustomExtensions.dtpShortDateMorningAfternoon:
                    string AMPM = "AM";
                    if (DateTime.Now.Hour >= 12)
                    {
                        AMPM = "Afternoon";
                    }
                    mvarCustomFormatMessage = "Short Date Morning/Afternoon (" + DateTime.Now.ToString("d", Culture) + " " + AMPM + ")";
                    this.CustomFormat = "MM-dd-yyyy HH:mm:ss";
                    break;
            }
            Tooltip.SetToolTip(txtDateTime, mvarCustomFormatMessage);
        }

        //Dispplay dates Times etc, based on Format selected
        private void FormatTextBox()
        {

            switch (mvarFormatEx)
            {
                case dtpCustomExtensions.dtpCustom:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString(this.CustomFormat, Culture);
                    break;
                case dtpCustomExtensions.dtpDayFullName:
                    try
                    {
                        this.Value = DateTime.Parse(txtDateTime.Text);
                    }
                    catch
                    {
                        int aDay;
                        DateTime aDate;
                        for (aDay = 1; aDay < 8; aDay++)
                        {
                            aDate = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-01-" + aDay.ToString(Culture));
                            if (aDate.DayOfWeek.ToString().ToLower() == txtDateTime.Text.ToLower() || aDate.DayOfWeek.ToString().Substring(0, 3).ToLower() == txtDateTime.Text.ToLower())
                            {
                                this.Value = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-01-" + aDay.ToString(Culture));
                                break;
                            }
                        }
                    }
                    txtDateTime.Text = this.Value.ToString("dddd", Culture);
                    break;
                case dtpCustomExtensions.dtpDayShortName:
                    try
                    {
                        this.Value = DateTime.Parse(txtDateTime.Text);
                    }
                    catch
                    {
                        int aDay;
                        DateTime aDate;
                        for (aDay = 1; aDay < 8; aDay++)
                        {
                            aDate = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-01-" + aDay.ToString(Culture));
                            if (aDate.DayOfWeek.ToString().ToLower() == txtDateTime.Text.ToLower() || aDate.DayOfWeek.ToString().Substring(0, 3).ToLower() == txtDateTime.Text.ToLower())
                            {
                                this.Value = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-01-" + aDay.ToString(Culture));
                                break;
                            }
                        }
                    }
                    txtDateTime.Text = this.Value.ToString("ddd", Culture);
                    break;
                case dtpCustomExtensions.dtpLongDateLongTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("D", Culture) + " " + this.Value.ToString("HH:mm:ss", Culture);
                    break;
                case dtpCustomExtensions.dtpLongDateLongTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("D", Culture) + " " + this.Value.ToString("hh:mm:ss tt", Culture);
                    break;
                case dtpCustomExtensions.dtpLongDateShortTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("D", Culture) + " " + this.Value.ToString("HH:mm", Culture);
                    break;
                case dtpCustomExtensions.dtpLongDateShortTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("D", Culture) + " " + this.Value.ToString("hh:mm tt", Culture);
                    break;
                case dtpCustomExtensions.dtpLongTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("HH:mm:ss", Culture);
                    break;
                case dtpCustomExtensions.dtpMonthFullName:
                    try
                    {
                        this.Value = DateTime.Parse(txtDateTime.Text);
                    }
                    catch
                    {
                        int aMonth;
                        DateTime aDate;
                        string[] sMonth = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                        for (aMonth = 0; aMonth < 12; aMonth++)
                        {
                            aDate = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-" + (aMonth + 1) + "-" + "01");
                            if (sMonth[aMonth].ToLower() == txtDateTime.Text.ToLower() || sMonth[aMonth].ToLower() == txtDateTime.Text.Substring(0, 3).ToLower())
                            {
                                this.Value = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-" + (aMonth + 1) + "-" + "01");
                                break;
                            }
                        }
                    }
                    txtDateTime.Text = this.Value.ToString("MMMM", Culture);
                    break;
                case dtpCustomExtensions.dtpMonthShortName:
                    try
                    {
                        this.Value = DateTime.Parse(txtDateTime.Text);
                    }
                    catch
                    {
                        int aMonth;
                        DateTime aDate;
                        string[] sMonth = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                        for (aMonth = 0; aMonth < 12; aMonth++)
                        {
                            aDate = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-" + (aMonth + 1) + "-" + "01");
                            if (sMonth[aMonth].ToLower() == txtDateTime.Text.ToLower() || sMonth[aMonth].ToLower() == txtDateTime.Text.Substring(0, 3).ToLower())
                            {
                                this.Value = DateTime.Parse(DateTime.Now.Year.ToString(Culture) + "-" + (aMonth + 1) + "-" + "01");
                                break;
                            }
                        }
                    }
                    txtDateTime.Text = this.Value.ToString("MMM", Culture);
                    break;
                case dtpCustomExtensions.dtpMonthNameAndDay:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("M", Culture);
                    break;
                case dtpCustomExtensions.dtpShortDateLongTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("d", Culture) + " " + this.Value.ToString("HH:mms:ss", Culture);
                    break;
                case dtpCustomExtensions.dtpShortDateLongTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("d", Culture) + " " + this.Value.ToString("hh:mms:ss tt", Culture);
                    break;
                case dtpCustomExtensions.dtpShortDateShortTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("d", Culture) + " " + this.Value.ToString("HH:mm", Culture);
                    break;
                case dtpCustomExtensions.dtpShortDateShortTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("d", Culture) + " " + this.Value.ToString("hh:mms tt", Culture);
                    break;
                case dtpCustomExtensions.dtpShortTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("HH:mm", Culture);
                    break;
                case dtpCustomExtensions.dtpShortTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("hh:mm tt", Culture);
                    break;
                case dtpCustomExtensions.dtpSortableDateAndTimeLocalTime:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("s", Culture);
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndLongTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("yyyy-MM-dd", Culture) + " " + this.Value.ToString("HH:mm:ss", Culture);
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndLongTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("yyyy-MM-dd", Culture) + " " + this.Value.ToString("hh:mm:ss tt", Culture);
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndShortTime24Hour:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("yyyy-MM-dd", Culture) + " " + this.Value.ToString("HH:mm", Culture);
                    break;
                case dtpCustomExtensions.dtpUTFLocalDateAndShortTimeAMPM:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("yyyy-MM-dd", Culture) + " " + this.Value.ToString("hh:mm tt", Culture);
                    break;
                case dtpCustomExtensions.dtpYear4Digit:
                    try
                    {
                        this.Value = DateTime.Parse(txtDateTime.Text);
                    }
                    catch
                    {
                        this.Value = DateTime.Parse("01 01 " + txtDateTime.Text);
                    }
                    txtDateTime.Text = this.Value.ToString("yyyy", Culture);
                    break;
                case dtpCustomExtensions.dtpYearAndMonthName:
                    try
                    {
                        this.Value = DateTime.Parse(txtDateTime.Text);
                    }
                    catch
                    {
                        try
                        {
                            txtDateTime.Text = DateTime.Now.Year.ToString() + " " + int.Parse(txtDateTime.Text, Culture).ToString();
                        }
                        catch
                        {
                            this.Value = DateTime.Parse(txtDateTime.Text + " 01");
                        }
                    }
                    txtDateTime.Text = this.Value.ToString("Y", Culture);
                    break;
                case dtpCustomExtensions.dtpShortDateAMPM:
                    if (txtDateTime.Text.Substring(txtDateTime.Text.Length - 2, 2).ToLower() == "pm")
                    {
                        txtDateTime.Text = txtDateTime.Text.Substring(0, txtDateTime.Text.Length - 2);
                        txtDateTime.Text = txtDateTime.Text + " 13:00";
                    }
                    else
                    {
                        if (txtDateTime.Text.Substring(txtDateTime.Text.Length - 2, 2).ToLower() == "am")
                        {
                            txtDateTime.Text = txtDateTime.Text.Substring(0, txtDateTime.Text.Length - 2);
                        }
                        txtDateTime.Text = txtDateTime.Text + " 01:00";
                    }
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToString("d", Culture) + " " + this.Value.ToString("tt", Culture);
                    break;
                case dtpCustomExtensions.dtpShortDateMorningAfternoon:
                    string AMPM = "Morning";
                    if (txtDateTime.Text.Substring(txtDateTime.Text.Length - 2, 2).ToLower() == "pm")
                    {
                        txtDateTime.Text = txtDateTime.Text.Substring(0, txtDateTime.Text.Length - 2);
                        txtDateTime.Text = txtDateTime.Text + " 13:00";
                    }
                    else
                    {
                        if (txtDateTime.Text.Substring(txtDateTime.Text.Length - 2, 2).ToLower() == "am")
                        {
                            txtDateTime.Text = txtDateTime.Text.Substring(0, txtDateTime.Text.Length - 2);
                        }
                        txtDateTime.Text = txtDateTime.Text + " 01:00";
                    }
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    if (this.Value.Hour >= 12)
                    {
                        AMPM = "Afternoon";
                    }
                    txtDateTime.Text = this.Value.ToString("d", Culture) + " " + AMPM;
                    break;
                case dtpCustomExtensions.dtpLong:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToLongDateString();
                    break;
                case dtpCustomExtensions.dtpShort:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToShortDateString();
                    break;
                case dtpCustomExtensions.dtpTime:
                    this.Value = DateTime.Parse(txtDateTime.Text);
                    txtDateTime.Text = this.Value.ToLongTimeString();
                    break;
                default:
                    break;
            }
        }

        #endregion

    }

}
