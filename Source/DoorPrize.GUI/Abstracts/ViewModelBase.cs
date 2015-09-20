using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using PropertyDictionary = System.Collections.Generic.Dictionary<
    string, System.ComponentModel.PropertyChangedEventHandler>;

namespace DoorPrize.GUI
{
    public abstract class ViewModelBase<TViewModel, TModel> :
        MvvmBase, INotifyPropertyChanged
        where TViewModel : ViewModelBase<TViewModel, TModel>
        where TModel : ModelBase<TModel>
    {
        private readonly Dictionary<string, PropertyDictionary>
            propertyHandlers = new Dictionary<string, PropertyDictionary>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected ViewModelBase(TModel model)
        {
            Model = model;
        }

        protected TModel Model { get; set; }

        protected virtual void AssociateProperties<TModelResult, TViewModelResult>(
            Expression<Func<TModel, TModelResult>> modelProperty,
            Expression<Func<TViewModel, TViewModelResult>> viewModelProperty)
        {
            var modelPropertyName =
                ((MemberExpression)modelProperty.Body).Member.Name;

            var viewModelPropertyName =
                ((MemberExpression)viewModelProperty.Body).Member.Name;

            if (!propertyHandlers.ContainsKey(modelPropertyName))
            {
                propertyHandlers.Add(modelPropertyName,
                    new PropertyDictionary());
            }

            var handlers = propertyHandlers[modelPropertyName];

            PropertyChangedEventHandler handler = (s, ea) =>
            {
                if (ea.PropertyName == modelPropertyName)
                {
                    NotifyPropertyChanged(viewModelPropertyName,
                           this, PropertyChanged);
                }
            };

            Model.PropertyChanged += handler;

            handlers.Add(viewModelPropertyName, handler);
        }

        private void NotifyPropertyChanged(string propertyName,
            object sender, PropertyChangedEventHandler propertyChanged)
        {
            if (propertyChanged == null)
                return;
            if (Dispatcher.CheckAccess())
            {
                propertyChanged(sender, new PropertyChangedEventArgs(
                    propertyName));
            }
            else
            {
                Action action = () => propertyChanged
                    (sender, new PropertyChangedEventArgs(propertyName));

                Dispatcher.BeginInvoke(action);
            }
        }

        protected virtual void NotifyPropertyChanged<TResult>(
            Expression<Func<TViewModel, TResult>> property)
        {
            BindingHelper.NotifyPropertyChanged(property, this,
                PropertyChanged, Dispatcher);
        }
    }
}
