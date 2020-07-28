using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using FirstBioRobApp.Controls;
using Plugin.BluetoothLE;
using FirstBioRobApp.Models;

namespace FirstBioRobApp.ViewModels
{
    public class BluetoothPageViewModel : BaseViewModel
    {
        public BluetoothPageViewModel()
        {
            Title = "Bluetooth Zone";
            CenterLabel = "HELLO COVID-19 WORLD!";
            Connect_Button_Clicked = new Command(Connect_Button_Clicked_Event);
            SearchNew_Button_Clicked = new Command(NewDevices_Button_Clicked_Event);
            DeviceList_Tapped = new Command(DeviceList_Tapped_Event);
            Scan_Devices_Button_BgColor = "#D6D7D7";
            Scan_Devices_Button_Text = "Scan nearby devices";
            _can_Conect = false;
            /*
             * 1 - Verify if bluetooth acess is allowed, if not, request to allow
             * 2 - Verify if bluetooth service is on, if not, request to turn on
             * 3 - Start scanning for devices
             */
            if (CrossBleAdapter.Current.Status == AdapterStatus.PoweredOff)
                CrossBleAdapter.Current.SetAdapterState(true); //TURN BLUETOOTH ON, False TURN OFF - Supported by Android only
        }


        #region Declarations

        public List<IDevice> iDeviceList = new List<IDevice>();
        public ObservableCollection<BluetoothPageModel> _bluetoothDevices = new ObservableCollection<BluetoothPageModel>();
        public string _centerLabel;
        public string _scan_Devices_Button_BgColor;
        public string _scan_Devices_Button_Text;
        public BluetoothPageModel _selectedItem;
        public bool _can_Conect;


        public string CenterLabel
        {
            get { return _centerLabel; }
            set { SetProperty(ref _centerLabel, value); }
        }
        public string Scan_Devices_Button_BgColor
        {
            get { return _scan_Devices_Button_BgColor; }
            set { SetProperty(ref _scan_Devices_Button_BgColor, value); }
        }
        public string Scan_Devices_Button_Text
        {
            get { return _scan_Devices_Button_Text; }
            set { SetProperty(ref _scan_Devices_Button_Text, value); }
        }
        public ObservableCollection<BluetoothPageModel> BluetoothDevices
        {
            get { return _bluetoothDevices; }
            set { SetProperty(ref _bluetoothDevices, value); }
        }
        public BluetoothPageModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }
        public bool Can_Connect
        {
            get { return _can_Conect; }
            set { SetProperty(ref _can_Conect, value); }
        }

        public ICommand Connect_Button_Clicked { get; private set; }
        public ICommand SearchNew_Button_Clicked { get; private set; }
        public ICommand DeviceList_Tapped { get; private set; }

        #endregion

        #region Functions

        private void NewDevices_Button_Clicked_Event(object obj)
        {
            // discover some devices
            if (!CrossBleAdapter.Current.IsScanning)
            {
                BluetoothDevices.Clear();
                iDeviceList.Clear();
                Scan_Devices_Button_BgColor = Color.Red.ToHex();
                Scan_Devices_Button_Text = "Scanning... Tap to stop!";
                var scanner = CrossBleAdapter.Current.Scan().Subscribe(scanResult =>
                {
                    if (!iDeviceList.Contains(scanResult.Device))
                    {
                        iDeviceList.Add(scanResult.Device);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            BluetoothDevices.Add(new BluetoothPageModel
                            {
                                device = scanResult,
                                distance = Math.Pow(10, ((-68 - scanResult.Rssi) / 31.1474))
                            });
                        });
                    }
                    else
                    {
                        int ind = iDeviceList.IndexOf(scanResult.Device);


                        Device.BeginInvokeOnMainThread(() =>
                        {
                            BluetoothDevices[ind].device = scanResult;
                            BluetoothDevices[ind].distance = Math.Pow(10, ((-68 - scanResult.Rssi) / 31.1474));
                            //BluetoothDevices[ind] = new BluetoothPageModel
                            //{
                            //    device = scanResult,
                            //    distance = Math.Pow(10, ((-68 - scanResult.Rssi) / 31.1474))
                            //};
                        });
                    }
                });
            }
            else
            {
                Scan_Devices_Button_BgColor = "#D6D7D7";
                Scan_Devices_Button_Text = "Scan nearby devices";
                CrossBleAdapter.Current.StopScan(); //When you want to stop scanning
            }
        }

        private void Connect_Button_Clicked_Event(object obj)
        {
            Scan_Devices_Button_BgColor = "#D6D7D7";
            Scan_Devices_Button_Text = "Scan nearby devices";
            CrossBleAdapter.Current.StopScan(); //When you want to stop scanning
            SelectedItem.device.Device.ConnectWait();
            
        }

        private void DeviceList_Tapped_Event(object obj)
        {

        }

        #endregion
    }
}
