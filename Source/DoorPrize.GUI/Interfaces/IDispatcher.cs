using System;
using System.Diagnostics.Contracts;

namespace DoorPrize.GUI
{
    [ContractClass(typeof(IDispatcherContract))]
    public interface IDispatcher
    {
        bool CheckAccess();
        void BeginInvoke(Action action);
    }

    [ContractClassFor(typeof(IDispatcher))]
    abstract class IDispatcherContract : IDispatcher
    {
        public bool CheckAccess()
        {
            return default(bool);
        }

        public void BeginInvoke(Action action)
        {
            Contract.Requires(action != null);
        }
    }
}
