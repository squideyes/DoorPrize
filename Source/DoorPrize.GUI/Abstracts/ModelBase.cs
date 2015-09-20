using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace DoorPrize.GUI
{
    public class ModelBase<TM> : MvvmBase, INotifyPropertyChanged
        where TM : ModelBase<TM>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged<TR>(
            Expression<Func<TM, TR>> property)
        {
            Contract.Requires(property != null);

            BindingHelper.NotifyPropertyChanged(
                property, this, PropertyChanged, Dispatcher);
        }
    }
}