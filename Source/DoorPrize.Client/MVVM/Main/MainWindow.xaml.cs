using DoorPrize.Shared;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Windows;

namespace DoorPrize.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MonitorChanges();
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
                    var body =  message.GetBody<DrawingInfo>();

                    message.Complete();
                }
                catch (Exception)
                {
                    message.Abandon();
                }
            },
            options);
        }
    }
}
