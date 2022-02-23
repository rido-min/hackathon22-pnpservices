using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ThingsApp.Models;

namespace ThingsApp.Controls
{
    public class DeviceTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement? element = container as FrameworkElement;

            if (element != null && item != null)
            {
                if ((item as BaseDevice).ModelId == "dtmi:pnd:demo:smartlightbulb;1")
                {
                    return element.FindResource("LightFixtureTemplate") as DataTemplate;
                }
                else
                {
                    return element.FindResource("UnknownDeviceTemplate") as DataTemplate;
                }
            }
            else
                return null;
        }
    }
}
