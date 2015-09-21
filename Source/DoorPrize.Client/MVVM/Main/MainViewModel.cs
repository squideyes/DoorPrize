using DoorPrize.Client.Helpers;
using DoorPrize.Client.Primatives;
using DoorPrize.GUI;
using DoorPrize.Shared;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
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

            MonitorChanges();
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

        private void MonitorChanges()
        {
            var connString = CloudConfigurationManager.
                GetSetting("Microsoft.ServiceBus.ConnectionString");

            var client = SubscriptionClient.CreateFromConnectionString(
                connString, WellKnown.TopicName, WellKnown.SubscriptionName);

            var options = new OnMessageOptions()
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };

            client.OnMessage((message) =>
            {
                try
                {
                    var body = message.GetBody<DrawingInfo>();

                    message.Complete();

                    Prizes = body.PrizesLeft;
                    Tickets = body.TicketsLeft;
                }
                catch (Exception error)
                {
                    // The error should be logged!!!!!!!!!!!!!!!!!!!!

                    message.Abandon();
                }
            },
            options);
        }
    }
}
