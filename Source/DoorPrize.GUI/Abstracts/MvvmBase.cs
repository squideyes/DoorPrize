using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace DoorPrize.GUI
{
    public abstract class MvvmBase 
    {
        protected readonly IDispatcher Dispatcher;

        protected MvvmBase()
        {
            Dispatcher = UiDispatcher.Current;
        }

        protected void Notify(EventHandler handler)
        {
            if (handler != null)
                InternalNotify(() => handler(this, new NotifyArgs()));
        }

        protected void Notify(EventHandler handler, NotifyArgs e)
        {
            if (handler != null)
                InternalNotify(() => handler(this, e));
        }

        protected void Notify(EventHandler<NotifyArgs> handler)
        {
            if (handler != null)
                InternalNotify(() => handler(this, new NotifyArgs()));
        }

        protected void Notify(EventHandler<NotifyArgs> handler, NotifyArgs e)
        {
            if (handler != null)
                InternalNotify(() => handler(this, e));
        }

        protected void Notify<T>(EventHandler<NotifyArgs<T>> handler,
            NotifyArgs<T> e)
        {
            if (handler != null)
                InternalNotify(() => handler(this, e));
        }

        private static void InternalNotify(Action method)
        {
            Contract.Requires(method != null);

            if (UiDispatcher.Current.CheckAccess())
                method();
            else
                UiDispatcher.Current.BeginInvoke(method);
        }
    }
}
