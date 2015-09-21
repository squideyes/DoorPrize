using DoorPrize.Shared;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DoorPrize.Client.Helpers
{
    public static class RestHelper
    {
        public static async Task<Tuple<int, int>> GetPrizesAndTicketsLeft()
        {
            Tuple<int, int> result;

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

                            result = new Tuple<int, int>(
                                drawingInfo.PrizesLeft, drawingInfo.TicketsLeft);
                        }
                    }
                }
            }

            return result;
        }
        public static async Task<WinnerInfo> GetWinnerInfo()
        {
            WinnerInfo winnerInfo;

            using (var handler = new HttpClientHandler { UseProxy = false })
            {
                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = Properties.Settings.Default.DrawingUri;

                    using (HttpResponseMessage response = await httpClient.GetAsync(
                        string.Format("{0}", Properties.Settings.Default.AccountPhone)))
                    {
                        using (HttpContent content = response.Content)
                        {
                            winnerInfo = JsonConvert.DeserializeObject<WinnerInfo>(
                                await content.ReadAsStringAsync());
                        }
                    }
                }
            }

            return winnerInfo;
        }
    }
}
