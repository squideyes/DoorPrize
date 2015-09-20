using System;
using System.Threading;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;

namespace DoorPrize.GUI
{
    public static class BindingHelper
    {
        public static void NotifyPropertyChanged<M, R>(this M model, Expression<Func<M, R>> property,
            PropertyChangedEventHandler propertyChanged, IDispatcher dispatcher)
        {
            Contract.Requires(model != null);
            Contract.Requires(property != null);
            Contract.Requires(dispatcher != null);

            var propertyName = ((MemberExpression)property.Body).Member.Name;

            InternalNotifyPropertyChanged(propertyName, model, propertyChanged, dispatcher);
        }

        public static void NotifyPropertyChanged<M, R>(Expression<Func<M, R>> property,
            object sender, PropertyChangedEventHandler propertyChanged, IDispatcher dispatcher)
        {
            Contract.Requires(property != null);
            Contract.Requires(sender!= null);
            Contract.Requires(dispatcher != null);

            var propertyName = ((MemberExpression)property.Body).Member.Name;

            InternalNotifyPropertyChanged(propertyName, sender, propertyChanged, dispatcher);
        }

        private static void InternalNotifyPropertyChanged(string propertyName,
            object sender, PropertyChangedEventHandler propertyChanged, IDispatcher dispatcher)
        {
            if (propertyChanged != null)
            {
                if (dispatcher.CheckAccess())
                {
                    propertyChanged(sender, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    dispatcher.BeginInvoke(() => propertyChanged(
                        sender, new PropertyChangedEventArgs(propertyName)));
                }
            }
        }
    }
}
