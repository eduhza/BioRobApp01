using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Globalization;
using FirstBioRobApp.Models;
using FirstBioRobApp.ViewModels;
using System.Collections;

namespace FirstBioRobApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotPage : ContentPage
    {
        public PlotPage()
        {
            InitializeComponent();
        } 
    }
}


/*

Task.Run(async () =>
{
    // Your code here
 
    Device.BeginInvokeOnMainThread(() =>
    {
        // UI interaction here
    });
})

 */
