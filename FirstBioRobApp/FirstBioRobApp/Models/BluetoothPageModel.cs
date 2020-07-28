using FirstBioRobApp.ViewModels;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirstBioRobApp.Models
{
    public class BluetoothPageModel:BaseViewModel
    {
        public IScanResult _device;
        public double _distance;
        public IScanResult device  
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }
        public double distance
        {
            get { return _distance; }
            set { SetProperty(ref _distance, value); }
        }
    }
}
