using Devkoes.Restup.WebServer.File;
using Devkoes.Restup.WebServer.Http;
using Devkoes.Restup.WebServer.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LegoTrain
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpServer webserver;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // first create the webserver
            await TrainManagement.InitTrain();
            webserver = new HttpServer(80);

            var restRouteHandler = new RestRouteHandler();
            try
            {
                webserver.RegisterRoute("file", new StaticFileRouteHandler(ApplicationData.Current.LocalFolder.Path + "\\file"));
                webserver.RegisterRoute("", restRouteHandler);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            restRouteHandler.RegisterController<TrainManagement>();

            await webserver.StartServerAsync();

            await TrainManagement.InitIoTHub();
        }
    }
}
