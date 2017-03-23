// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/01/16, 12:26 PM
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhaghen.Commons.dll
// ------------------------------------------------------------------------

namespace WB.Commons.WPF.Converters
{
    using System;
    using System.Diagnostics;
    using System.Windows.Data;

    public class DebugConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                Debug.WriteLine(string.Format(culture, "Value: {0}", value));
            else
                Debug.WriteLine("Value is null");

            if (parameter != null)
                Debug.WriteLine(string.Format(culture, "Parameter: {0}", parameter));

            return targetType;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}