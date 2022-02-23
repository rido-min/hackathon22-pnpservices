using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThingsApp.Models;

namespace ThingsApp
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BaseDevice>? devices;
        public ObservableCollection<BaseDevice> Devices
        {
            get => devices;
            set
            {
                devices = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            Devices = await App.IotPlatform.GetDevicesAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
