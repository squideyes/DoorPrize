using System;
using System.Threading.Tasks;

namespace DoorPrize.Services
{
    public static class MiscExtenders
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                // log errors
            }
        }
    }
}