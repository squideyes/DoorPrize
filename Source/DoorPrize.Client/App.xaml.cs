using DoorPrize.Client.MVVM.Main;
using DoorPrize.GUI;
using System;
using System.Windows;

namespace DoorPrize.Client
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs sea)
        {
            DispatcherUnhandledException += (s, e) => 
                Modal.FailureDialog(e.Exception.Message);

            try
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose;

                MainRunner.Run();
            }
            catch (Exception error)
            {
                Modal.FailureDialog(error.Message);
            }
        }
    }
}
