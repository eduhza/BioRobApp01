using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FirstBioRobApp.Views;

namespace FirstBioRobApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public ICommand OpenURL => new Command<string>(async (url) => await Launcher.OpenAsync(url));
    }
}
