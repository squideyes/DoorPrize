﻿using DoorPrize.Services.Helpers;
using DoorPrize.Services.Models;
using DoorPrize.Shared;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace DoorPrize.Services.Controllers
{
    public class TwilioController : ApiController
    {
        private static Regex phoneRegex = new Regex(@"^\+1\d{10}$", RegexOptions.Compiled);

        private static Regex emailRegex = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]FormDataCollection formData)
        {
            const string BADPHONE = "An SMS message with an invalid \"{0}\" number was received!";

            const string REGISTERED =
                "You're registered to win a door prize in {0}'s {1:MM/dd/yyyy} drawing.  Good luck!!";

            const string DRAWINGCLOSED =
                "{0}'s {1:MM/dd/yyyy} door prize drawing is closed and can accept no more entries!";

            const string BADBODY =
                "To register to win a door prize, please send an SMS in the format EMAIL,NAME (i.e. somedude@someco.com,Some Dude) to {0}.";

            var to = formData["To"];

            if (!phoneRegex.IsMatch(to))
                return GetResponse(BADPHONE, "To");

            var from = formData["From"];

            if (!phoneRegex.IsMatch(from))
                return GetResponse(BADPHONE, "From");

            var body = formData["Body"];

            if (string.IsNullOrWhiteSpace(body))
                return GetResponse(BADBODY, from);

            var parts = body.Split(',', ';', '|');

            if (parts.Length != 2)
                return GetResponse(BADBODY, from);

            var email = parts[0].Trim().ToLower();
            var name = parts[1].Trim().ToUpper();

            if (!emailRegex.IsMatch(email))
                return GetResponse(BADBODY, from);

            if (string.IsNullOrWhiteSpace(name))
                return GetResponse(BADBODY, from);

            to = to.Substring(2);
            from = from.Substring(2);

            using (var db = new Entities())
            {
                var account = await db.Accounts.FirstOrDefaultAsync(a => a.Phone == to);

                if (account == null)
                    return GetResponse(BADPHONE, "To");

                var drawing = await db.Drawings.FirstOrDefaultAsync(
                    c => c.AccountId == account.Id && c.Date == DateTime.Today);

                if (drawing == null)
                {
                    drawing = new Drawing()
                    {
                        AccountId = account.Id,
                        Date = DateTime.Today
                    };

                    db.Drawings.Add(drawing);

                    await db.SaveChangesAsync();
                }

                if (await db.Winners.AnyAsync(w => w.Prize.DrawingId == drawing.Id))
                    return GetResponse(DRAWINGCLOSED, account.Name, drawing.Date);

                var ticket = await db.Tickets.FirstOrDefaultAsync(t =>
                    t.DrawingId == drawing.Id && t.Phone == from);

                if (ticket == null)
                {
                    ticket = new Ticket()
                    {
                        DrawingId = drawing.Id,
                        Email = email,
                        Name = name,
                        Phone = from
                    };

                    db.Tickets.Add(ticket);
                }
                else
                {
                    ticket.Name = name;
                    ticket.Email = email;
                }

                await db.SaveChangesAsync();

                int prizesLeft = 0;
                int ticketsLeft = 0;

                var prizes = await (from p in db.Prizes
                                    where p.DrawingId == drawing.Id
                                    select new
                                    {
                                        Quantity = p.Quantity - p.Winners.Count()
                                    }).ToListAsync();

                foreach (var prize in prizes)
                {
                    for (int i = 0; i < prize.Quantity; i++)
                        prizesLeft++;
                }

                ticketsLeft = await (from t in db.Tickets
                                     where t.DrawingId == drawing.Id && t.Winners.Count == 0
                                     select t).CountAsync();

#pragma warning disable CS4014
                Task.Run(() => PublishDrawingInfo(drawing, prizesLeft, ticketsLeft));
#pragma warning restore CS414

                return GetResponse(REGISTERED, account.Name, drawing.Date);
            }
        }

        private HttpResponseMessage GetResponse(string format, params object[] args)
        {
            var xml = new XDocument(new XElement("Response",
                new XElement("Sms", string.Format(format, args))));

            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent(
                xml.ToString(), Encoding.UTF8, "application/xml");

            return response;
        }

        private void PublishDrawingInfo(Drawing drawing, int prizesLeft, int ticketsLeft)
        {
            var message = new BrokeredMessage(new DrawingInfo()
            {
                AccountName = drawing.Account.Name,
                AccountPhone = drawing.Account.Phone,
                DrawingDate = drawing.Date,
                PrizesLeft = prizesLeft,
                TicketsLeft = ticketsLeft
            });

            BusHelper.GetTopicClient().SendAsync(message);
        }
    }
}
