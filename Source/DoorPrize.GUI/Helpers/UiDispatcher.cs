using System;
using System.Diagnostics.Contracts;
using WindowsDispatcher = System.Windows.Threading.Dispatcher;

namespace DoorPrize.GUI
{
    public sealed class UiDispatcher : IDispatcher
    {
        private static volatile IDispatcher dispatcher;

        private static readonly object syncRoot = new object();
        
        private readonly WindowsDispatcher windowsDispatcher;

        private UiDispatcher(WindowsDispatcher windowsDispatcher)
        {
            this.windowsDispatcher = windowsDispatcher;
        }

        public bool CheckAccess()
        {
            return windowsDispatcher.CheckAccess();
        }

        public void BeginInvoke(Action action)
        {
            windowsDispatcher.BeginInvoke(action);
        }

        public static IDispatcher Current
        {
            get
            {
                if (dispatcher != null) 
                    return dispatcher;

                lock (syncRoot)
                {
                    dispatcher = new UiDispatcher(
                        WindowsDispatcher.CurrentDispatcher);
                }

                return dispatcher;
            }
        }
    }
}
