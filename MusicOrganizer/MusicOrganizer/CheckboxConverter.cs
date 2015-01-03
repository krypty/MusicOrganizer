using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicOrganizer
{
    public class CheckboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("value: " + value);

            //TODO: changer tag: mettre objet Nom + isChecked (bool)

            CheckboxTag cbxTag = (CheckboxTag)value;

            cbxTag.IsChecked = !cbxTag.IsChecked;

            return cbxTag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("parameter: " + parameter);
            return new CheckboxTag("newTag", (bool)value);
        }
    }
}
