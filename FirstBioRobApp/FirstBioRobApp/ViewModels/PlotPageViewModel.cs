using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using FirstBioRobApp.Models;
using FirstBioRobApp.Views;
using System.Linq;
using Syncfusion.SfChart.XForms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FirstBioRobApp.ViewModels
{
    public class PlotPageViewModel : BaseViewModel
    {
        //Initialization
        public PlotPageViewModel()
        {
            CircularBuffer = new ObservableCollection<PlotPageModel>();
            Title = "Graph Zone";
            Mean_Label = 0.ToString("#0.00");
            ChangeData_Clicked = new Command(LiveData_Button_Clicked);
            ChangeData_SfLabel = "Turn on Live Data";
            IsSfButton_Enabled = true;
            LoadedData = false;
            SeriesCollection = new ChartSeriesCollection();
            PopulateData(CircularBufferSize);
            GraphUpdateServiceRunning = false;
            SfButtonBackgroundColor = "#213347";


            DataStreaming_thread = new Thread(new ThreadStart(DataStreamingService));
            DataStreaming_thread.Priority = ThreadPriority.Highest;
            DataStreaming_thread.Start();

            //Task.Run(()=> { Update_Graph_Series(); });
            //Device.StartTimer(TimeSpan.FromMilliseconds(20), () =>
            //{
            //    Update_Graph_Series();
            //    return true; // True = Repeat again, False = Stop the timer
            //});

            //DataStreamingService();

            ////STREAMING SERVICE WILL BE ALWAYS ON (every 25milliseconds), BUT WILL ONLY UPDATE BUFFER WHEN FLAG IS ON
            //Device.StartTimer(TimeSpan.FromMilliseconds(25), () =>
            //{
            //    DataStreamingService();
            //    return true; // True = Repeat again, False = Stop the timer
            //});
        }

        #region Declarations

        //Attributes
        public ObservableCollection<PlotPageModel> CircularBuffer { get; set; }
        public ObservableCollection<PlotPageModel> tempBuffer;
        private string _mean_Label;
        private string _changeData_SfLabel;
        private bool _isSfButton_Enabled;
        private bool _loadedData;
        private string _sfButtonBackgroundColor;
        private string _sfBusyIndicatorBackgroundColor;
        private bool _graphUpdateServiceRunning;
        private ChartSeriesCollection _seriesCollection;

        //Public variables/Commands
        public Random rnd = new Random();
        public int CircularBufferSize = 1000;
        public int testIncrement = 0;
        public bool isNewData = false;
        Thread Chart_thread;
        Thread DataStreaming_thread; //Made it global to finish tread on close app event
        public string Mean_Label
        {
            get { return _mean_Label; }
            set { SetProperty(ref _mean_Label, value); }
        }
        public string ChangeData_SfLabel
        {
            get { return _changeData_SfLabel; }
            set { SetProperty(ref _changeData_SfLabel, value); }
        }
        public bool IsSfButton_Enabled
        {
            get { return _isSfButton_Enabled; }
            set { SetProperty(ref _isSfButton_Enabled, value); }
        }
        public bool LoadedData
        {
            get { return _loadedData; }
            set { SetProperty(ref _loadedData, value); }
        }
        public bool GraphUpdateServiceRunning
        {
            get { return _graphUpdateServiceRunning; }
            set { SetProperty(ref _graphUpdateServiceRunning, value); }
        }
        public string SfBusyIndicatorBackgroundColor
        {
            get { return _sfBusyIndicatorBackgroundColor; }
            set { SetProperty(ref _sfBusyIndicatorBackgroundColor, value); }
        }
        public string SfButtonBackgroundColor
        {
            get { return _sfButtonBackgroundColor; }
            set { SetProperty(ref _sfButtonBackgroundColor, value); }
        }
        public ChartSeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }
        public ICommand ChangeData_Clicked { get; private set; }

        #endregion


        #region Functions

        private void LiveData_Button_Clicked(object obj)
        {
            if (!GraphUpdateServiceRunning) //if data is not being updated, start updating!
            {
                GraphUpdateServiceRunning = !GraphUpdateServiceRunning;
                Chart_thread = new Thread(new ThreadStart(Update_Graph_Series));
                Chart_thread.Priority = ThreadPriority.Highest;
                Chart_thread.Start();

                ChangeData_SfLabel = "Updating data...";
                SfBusyIndicatorBackgroundColor = Color.Red.ToHex();
                SfButtonBackgroundColor = Color.Red.ToHex();
            }
            else //if data is being updated, stop updating!
            {
                GraphUpdateServiceRunning = !GraphUpdateServiceRunning;
                ChangeData_SfLabel = "Turn on Live Data";
                SfButtonBackgroundColor = "#213347";
            }

            //Initialize FPS counter
            int secondsPassed = 0;
            fps_Counter = 0;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Mean_Label = $"{ fps_Counter } - { secondsPassed }";//.ToString("#0.00");
                fps_Counter = 0;

                if (!GraphUpdateServiceRunning) //If not updating data, stop updating label
                    return false;

                secondsPassed++;
                return true; 
            });

           

            IsSfButton_Enabled = !IsSfButton_Enabled; //Activate button
        }

        Stopwatch stopwatch = new Stopwatch();
        void DataStreamingService()
        {
            int index = Int32.Parse(CircularBuffer.Last().XData);
            int tempInt = 0;
            while (true)
            {
                for (int i = 0; i < 25; i++)
                {
                    index++;
                    CircularBuffer.RemoveAt(0);

                    if (tempInt == null)
                        tempInt = 0;
                    CircularBuffer.Add(new Models.PlotPageModel()
                    {
                        XData = (index).ToString(),
                        YData = rnd.Next(-100, 100)
                    });
                }
                isNewData = true;
                Thread.Sleep(25);

                //Mean_Label = $"{ testIncrement }";

            }
        }


        public int fps_Counter = 0;
        void Update_Graph_Series()
        {
            while (GraphUpdateServiceRunning)
            {
                if (isNewData)
                {
                    isNewData = false;
                    FastLineSeries newSeries = UpdatePlotBuffer();
                    if (newSeries != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                SeriesCollection[0] = newSeries;
                                fps_Counter++;
                            }
                            catch 
                            {

                            } //NullReferenceException VERY OFTEN!!!!!!!!!!!!
                        });
                    }
                    //Mean_Label = $"{Convert.ToString(fps_Counter)} - { stopwatch.Elapsed.TotalMilliseconds.ToString("#0.0000") }";//.ToString("#0.00");
                }
                Thread.Sleep(5);
            }
        }

        private FastLineSeries UpdatePlotBuffer()
        {
            try
            {
                PlotPageModel[] PlotBuffer = new PlotPageModel[CircularBufferSize]; 
                CircularBuffer.CopyTo(PlotBuffer, 0);
                
                ObservableCollection<PlotPageModel> TempData = new ObservableCollection<PlotPageModel>();
                foreach (PlotPageModel temp in PlotBuffer)
                    TempData.Add(temp);

                FastLineSeries tempSeries = new FastLineSeries()
                {
                    ItemsSource = TempData,
                    XBindingPath = "XData",
                    YBindingPath = "YData",
                    Color = Color.Red,
                    EnableAntiAliasing = false,
                    EnableTooltip = false,
                    ShowTrackballInfo = false,
                    StrokeWidth = 1
                };
                return tempSeries;
            }
            catch (Exception e)
            {
                Exception a = e;
                return null;
            }
        }

        private void PopulateData(int num)
        {
            if (_loadedData)
                CircularBuffer.Clear();

            for (int i = 0; i < num; i++)
            {
                CircularBuffer.Add(new Models.PlotPageModel()
                {
                    XData = i.ToString(),
                    YData = rnd.Next(-100, 100)
                });
            }
            _loadedData = true;

            SeriesCollection = new ChartSeriesCollection();
            SeriesCollection.Add(UpdatePlotBuffer());
        }

        #endregion
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
