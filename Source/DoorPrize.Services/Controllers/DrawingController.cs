using DoorPrize.Services.Models;
using DoorPrize.Services.Primatives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace DoorPrize.Services.Controllers
{
    [RoutePrefix("api/Drawing")]
    public class DrawingController : ApiController
    {
        private class PrizeInfo
        {
            public Guid Guid { get; set; }
            public string Name { get; set; }
            public string Provider { get; set; }
            public int PrizeId { get; set; }
        }

        private static Regex accountPhoneRegex = new Regex(@"^\d{10}$", RegexOptions.Compiled);

        [HttpGet, Route("{accountPhone}")]
        public async Task<IHttpActionResult> Get(string accountPhone)
        {
            if (!accountPhoneRegex.IsMatch(accountPhone))
                return BadRequest("The \"accountPhone\" must be a 10-digit phone number.");

            var today = DateTime.Today;

            using (var db = new Entities())
            {
                var account = db.Accounts.FirstOrDefault(a => a.Phone == accountPhone);

                if (account == null)
                {
                    return BadRequest(string.Format(
                        "The \"{0}\" accountPhone is invalid!", accountPhone));
                }

                var drawing = db.Drawings.FirstOrDefault(d =>
                    d.Account.Phone == accountPhone && d.Date == today);

                if (drawing == null)
                {
                    return BadRequest(string.Format(
                        "There is no {0} {1:MM/dd/yyyy} drawing.", account.Name, today));
                }

                var prizes = from p in db.Prizes
                             where p.DrawingId == drawing.Id
                             select new
                             {
                                 PrizeId = p.Id,
                                 PrizeName = p.Name,
                                 Provider = p.Provider,
                                 Quantity = p.Quantity - p.Winners.Count()
                             };

                var prizeInfos = new List<PrizeInfo>();

                foreach (var prize in prizes)
                {
                    for (int i = 0; i < prize.Quantity; i++)
                    {
                        prizeInfos.Add(new PrizeInfo()
                        {
                            Guid = Guid.NewGuid(),
                            Name = prize.PrizeName,
                            PrizeId = prize.PrizeId,
                            Provider = prize.Provider
                        });
                    }
                }

                if (prizeInfos.Count == 0)
                    return Ok<WinnerInfo>(null);

                var random = new Random();

                var tickets = (from t in db.Tickets
                               where t.DrawingId == drawing.Id && t.Winners.Count == 0
                               select t).ToList();

                if (tickets.Count == 0)
                    return Ok<WinnerInfo>(null);

                var ticket = tickets[random.Next(tickets.Count)];

                var prizeInfo = prizeInfos.OrderBy(i =>
                    i.Guid).ToList()[random.Next(prizeInfos.Count)];

                var winner = new Winner()
                {
                    PrizeId = prizeInfo.PrizeId,
                    TicketId = ticket.Id,
                    WonOn = DateTime.UtcNow
                };

                db.Winners.Add(winner);

                await db.SaveChangesAsync();

                var winnerInfo = new WinnerInfo()
                {
                    Prize = prizeInfo.Name,
                    Provider = prizeInfo.Provider,
                    Name = ticket.Name,
                    Email = ticket.Email,
                    Phone = ticket.Phone,
                    Remaining = prizeInfos.Count - 1
                };

                return Ok(winnerInfo);
            }
        }
    }
}
