using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FirstBioRobApp.ViewModels
{
    public class AboutUsViewModel : BaseViewModel
    {
        public AboutUsViewModel()
        {
            Title = "About Us Zone";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://biorob.com.br"));
        }

        public ICommand OpenWebCommand { get; }
    }
}
