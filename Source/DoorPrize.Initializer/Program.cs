using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DoorPrize.Initializer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var startedOn = DateTime.UtcNow;

                using (var handler = new HttpClientHandler { UseProxy = false })
                {
                    using (var client = new HttpClient(handler))
                    {
                        client.BaseAddress = Properties.Settings.Default.Uri;
                        using (var reader = new StringReader(Properties.Resources.Data))
                        {
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                var fields = line.Split(',');

                                var phone = fields[0];
                                var email = fields[1];
                                var name = fields[2];

                                AsyncContext.Run(() => PostData(client, phone, email, name));

                                Console.WriteLine("Posted {0}", line);
                            }
                        }
                    }
                }

                var elapsed = DateTime.UtcNow - startedOn;

                Console.WriteLine();
                Console.WriteLine("Elapsed: " + elapsed);
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + error.Message);
            }

            Console.WriteLine();
            Console.Write("Press any key to terminate the program...");

            Console.ReadKey(true);
        }

        private static async Task PostData(HttpClient client, string phone, string email, string name)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("From", phone),
                new KeyValuePair<string, string>("To", Properties.Settings.Default.AccountPhone),
                new KeyValuePair<string, string>("Body", string.Format("{0},{1}", email, name))
            });

            var response = await client.PostAsync("/api/Twilio", content);

            var element = XElement.Parse(response.Content.ReadAsStringAsync().Result);

            if (!element.Element("Sms").Value.StartsWith("You're registered to win a door prize"))
                throw new Exception(element.Element("Sms").Value);
        }
    }
}