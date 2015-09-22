using DoorPrize.Shared;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace DoorPrize.Services.Helpers
{
    public static class BusHelper
    {
        private static TopicClient client = null;

        public static TopicClient GetTopicClient()
        {
            if (client == null)
            {
                var connString = CloudConfigurationManager.
                    GetSetting("Microsoft.ServiceBus.ConnectionString");

                client = TopicClient.CreateFromConnectionString(
                    connString, WellKnown.TopicName);
            }

            return client;
        }
    }
}