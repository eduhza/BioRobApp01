using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstBioRobApp
{
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjg0OTQyQDMxMzgyZTMyMmUzMGYxSVNCbzNVTmsyRWJJMjhwdzhTTkd4WW5tTXpvcVNQa2d1TmtnSXg5QnM9");

            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
