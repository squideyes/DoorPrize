using DoorPrize.Generic;
using System.Reflection;
using System.Windows;

namespace DoorPrize.GUI
{
    public static class Modal
    {
        private static readonly AppInfo appInfo;

        static Modal()
        {
            appInfo = new AppInfo(Assembly.GetEntryAssembly());
        }

        public static void FailureDialog(string format, 
            params object[] args)
        {
            MessageBox.Show(string.Format(format, args),
                appInfo.GetTitle(),
                MessageBoxButton.OK, MessageBoxImage.Exclamation,
                MessageBoxResult.OK, MessageBoxOptions.None);
        }
    }
}
