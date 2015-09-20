using DoorPrize.Client.Helpers;
using DoorPrize.Client.Primatives;
using DoorPrize.GUI;
using System.ComponentModel;

namespace DoorPrize.Client.MVVM.Main
{
    public class MainViewModel : ViewModelBase<MainViewModel, MainModel>
    {
        private int prizes;
        private int tickets;

        public MainViewModel(MainModel model)
            : base(model)
        {
            GridInfos = new BindingList<GridInfo>();
        }

        public BindingList<GridInfo> GridInfos { get; }

        public string Title
        {
            get
            {
                return Global.AppInfo.GetTitle();
            }
        }

        public string DisplayPhone
        {
            get
            {
                return Global.DisplayPhone;
            }
        }

        public int Prizes
        {
            get
            {
                return prizes;
            }
            set
            {
                prizes = value;

                NotifyPropertyChanged(vm => vm.Prizes);
                NotifyPropertyChanged(vm => vm.DrawCommand);
            }
        }

        public int Tickets
        {
            get
            {
                return tickets;
            }
            set
            {
                tickets = value;

                NotifyPropertyChanged(vm => vm.Tickets);
            }
        }

        public DelegateCommand DrawCommand
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                    },
                    () => Prizes > 0);
            }
        }

        public DelegateCommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                    });
            }
        }
    }
}
