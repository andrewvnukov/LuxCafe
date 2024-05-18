using Cafe_Managment.Model;
using Cafe_Managment.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cafe_Managment.Utilities
{
    class OrderActiveTooLongConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChequeModel cheque)
            {
                bool isActiveTooLong = (DateTime.Now - cheque.CreatedAt).TotalMinutes > 1;
                Debug.WriteLine($"Cheque Id: {cheque.Id}, IsActiveTooLong: {isActiveTooLong}");
                return isActiveTooLong;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
