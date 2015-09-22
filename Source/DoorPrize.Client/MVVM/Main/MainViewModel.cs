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
using System.Threading.Tasks;
using System.Windows.Media;

namespace DoorPrize.Client.MVVM.Main
{
    public class MainViewModel : ViewModelBase<MainViewModel, MainModel>
    {
        private int prizesLeft;
        private int ticketsLeft;

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

        public int PrizesLeft
        {
            get
            {
                return prizesLeft;
            }
            set
            {
                prizesLeft = value;

                NotifyPropertyChanged(vm => vm.PrizesLeft);
                NotifyPropertyChanged(vm => vm.DrawCommand);
            }
        }

        public int TicketsLeft
        {
            get
            {
                return ticketsLeft;
            }
            set
            {
                ticketsLeft = value;

                NotifyPropertyChanged(vm => vm.TicketsLeft);
            }
        }

        private GridInfo GetGridInfo(WinnerInfo info, Color background)
        {
            return new GridInfo()
            {
                Email = info.TicketEmail,
                Name = info.TicketName,
                Phone = info.TicketPhone,
                Prize = string.Format("{0} ({1})",
                info.PrizeName, info.PrizeProvider),
                Background = background
            };
        }

        public DelegateCommand DrawCommand
        {
            get
            {
                return new DelegateCommand(
                    async () =>
                    {
                        var winnerInfo = await RestHelper.GetWinnerInfo();

                        foreach (var gridInfo in GridInfos)
                            gridInfo.Background = Colors.White;

                        GridInfos.Add(GetGridInfo(winnerInfo, Colors.Cyan));

                        await UpdatePrizesAndTicketsLeft();

                        NotifyPropertyChanged(vm => vm.DrawCommand);
                    },
                    () => (PrizesLeft > 0) && (TicketsLeft > 0));
            }
        }

        public DelegateCommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(
                    async () =>
                    {
                        GridInfos.Clear();

                        var winnerInfos = await RestHelper.GetWinnerInfos();

                        for (int i = 0; i < winnerInfos.Count; i++)
                        {
                            GridInfos.Add(GetGridInfo(winnerInfos[i],
                                i == winnerInfos.Count - 1 ? Colors.Cyan : Colors.White));
                        }

                        await UpdatePrizesAndTicketsLeft();

                        NotifyPropertyChanged(vm => vm.DrawCommand);
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

                    await UpdatePrizesAndTicketsLeft();
                }
                catch (Exception error)
                {
                    // The error should be logged!!!!!!!!!!!!!!!!!!!!

                    message.Abandon();
                }
            },
            options);
        }

        private async Task UpdatePrizesAndTicketsLeft()
        {
            var tuple = await RestHelper.GetPrizesAndTicketsLeft();

            PrizesLeft = tuple.Item1;
            TicketsLeft = tuple.Item2;
        }
    }
}
