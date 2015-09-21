using DoorPrize.Client.Helpers;
using DoorPrize.Client.Primatives;
using DoorPrize.GUI;
using DoorPrize.Shared;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net.Http;

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

            var busClient = SubscriptionClient.CreateFromConnectionString(
                connString, WellKnown.TopicName, WellKnown.SubscriptionName);

            var options = new OnMessageOptions()
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };

            busClient.OnMessage(async (message) =>
            {
                try
                {
                    var body = message.GetBody<DrawingInfo>();

                    message.Complete();

                    using (var handler = new HttpClientHandler { UseProxy = false })
                    {
                        using (var httpClient = new HttpClient(handler))
                        {
                            httpClient.BaseAddress = Properties.Settings.Default.DrawingUri;

                            using (HttpResponseMessage response = await httpClient.GetAsync(
                                string.Format("{0}/Info", Properties.Settings.Default.AccountPhone)))
                            {
                                using (HttpContent content = response.Content)
                                {
                                    var drawingInfo = JsonConvert.DeserializeObject<DrawingInfo>(
                                        await content.ReadAsStringAsync());

                                    Prizes = drawingInfo.PrizesLeft;
                                    Tickets = drawingInfo.TicketsLeft;
                                }
                            }
                        }
                    }
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
