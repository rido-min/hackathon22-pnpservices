using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThingsApp.Models;

namespace ThingsApp.Controls
{
    /// <summary>
    /// Interaction logic for LightFixture.xaml
    /// </summary>
    public partial class LightFixtureControl : UserControl
    {
        public LightFixtureControl()
        {
            InitializeComponent();
        }
    }

    [ValueConversion(typeof(bool), typeof(Uri))]
    public class StateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool reportedState = (bool)value;
            if (reportedState)
            {
                return new Uri("../Assets/bulb_on.png", UriKind.Relative);
            }
            else
            {
                return new Uri("../Assets/bulb_off.png", UriKind.Relative);
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
