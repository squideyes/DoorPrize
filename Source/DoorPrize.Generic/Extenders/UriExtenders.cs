using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        public static async Task<HttpStatusCode> DownloadFile(
            this Uri uri, string fileName, HttpClient client = null)
        {
            fileName.EnsurePathExists();

            if (client == null)
                client = new HttpClient();

            var response = await client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            var contentStream = 
                await response.Content.ReadAsStreamAsync();

            using (var fileStream = new FileStream(
                fileName, FileMode.Create, FileAccess.Write))
            {
                await contentStream.CopyToAsync(fileStream);
            }

            return response.StatusCode;
        }
    }
}
