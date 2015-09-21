using DoorPrize.Services.Models;
using DoorPrize.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public async Task<IHttpActionResult> GetWinners(string accountPhone)
        {
            if (!accountPhoneRegex.IsMatch(accountPhone))
                return BadRequest("The \"accountPhone\" must be a 10-digit phone number.");

            var today = DateTime.Today;

            using (var db = new Entities())
            {
                var account = await db.Accounts.FirstOrDefaultAsync(a => a.Phone == accountPhone);

                if (account == null)
                {
                    return BadRequest(string.Format(
                        "The \"{0}\" accountPhone is invalid!", accountPhone));
                }

                var drawing = await db.Drawings.FirstOrDefaultAsync(d =>
                    d.Account.Phone == accountPhone && d.Date == today);

                if (drawing == null)
                {
                    return BadRequest(string.Format(
                        "There is no {0} {1:MM/dd/yyyy} drawing.", account.Name, today));
                }

                var tuple = await GetPrizesAndTicketsLeft(db, drawing.Id);

                var q = from w in db.Winners
                        where w.Prize.Drawing.AccountId == account.Id
                        select new WinnerInfo
                        {
                            PrizeName = w.Prize.Name,
                            PrizeProvider = w.Prize.Provider,
                            TicketName = w.Ticket.Name,
                            TicketEmail = w.Ticket.Email,
                            TicketPhone = w.Ticket.Phone,
                            AccountPhone = account.Phone,
                            AccountName = account.Name,
                            DrawingDate = drawing.Date,
                            PrizesLeft = tuple.Item1,
                            TicketsLeft = tuple.Item2
                        };

                return Ok(q.ToList());
            }
        }

        [HttpGet, Route("{accountPhone}/Info")]
        public async Task<IHttpActionResult> GetInfo(string accountPhone)
        {
            if (!accountPhoneRegex.IsMatch(accountPhone))
                return BadRequest("The \"accountPhone\" must be a 10-digit phone number.");

            var today = DateTime.Today;

            using (var db = new Entities())
            {
                var account = await db.Accounts.FirstOrDefaultAsync(a => a.Phone == accountPhone);

                if (account == null)
                {
                    return BadRequest(string.Format(
                        "The \"{0}\" accountPhone is invalid!", accountPhone));
                }

                var drawing = await db.Drawings.FirstOrDefaultAsync(d =>
                    d.Account.Phone == accountPhone && d.Date == today);

                if (drawing == null)
                {
                    return BadRequest(string.Format(
                        "There is no {0} {1:MM/dd/yyyy} drawing.", account.Name, today));
                }

                var tuple = await GetPrizesAndTicketsLeft(db, drawing.Id);

                var drawingInfo = new DrawingInfo()
                {
                    AccountPhone = account.Phone,
                    AccountName = account.Name,
                    DrawingDate = drawing.Date,
                    PrizesLeft = tuple.Item1,
                    TicketsLeft = tuple.Item2
                };

                return Ok(drawingInfo);
            }
        }

        [HttpGet, Route("{accountPhone}/Winner")]
        public async Task<IHttpActionResult> Get(string accountPhone)
        {
            if (!accountPhoneRegex.IsMatch(accountPhone))
                return BadRequest("The \"accountPhone\" must be a 10-digit phone number.");

            var today = DateTime.Today;

            using (var db = new Entities())
            {
                var account = await db.Accounts.FirstOrDefaultAsync(a => a.Phone == accountPhone);

                if (account == null)
                {
                    return BadRequest(string.Format(
                        "The \"{0}\" accountPhone is invalid!", accountPhone));
                }

                var drawing = await db.Drawings.FirstOrDefaultAsync(d =>
                    d.Account.Phone == accountPhone && d.Date == today);

                if (drawing == null)
                {
                    return BadRequest(string.Format(
                        "There is no {0} {1:MM/dd/yyyy} drawing.", account.Name, today));
                }

                var prizes = await (from p in db.Prizes
                                    where p.DrawingId == drawing.Id
                                    select new
                                    {
                                        PrizeId = p.Id,
                                        PrizeName = p.Name,
                                        Provider = p.Provider,
                                        Quantity = p.Quantity - p.Winners.Count()
                                    }).ToListAsync();

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

                var tickets = await (from t in db.Tickets
                                     where t.DrawingId == drawing.Id && t.Winners.Count == 0
                                     select t).ToListAsync();

                if (tickets.Count == 0)
                    return Ok<WinnerInfo>(null);

                var ticket = tickets[random.Next(tickets.Count)];

                var prizeInfo = prizeInfos.
                    OrderBy(i => i.Guid).ToList()[random.Next(prizeInfos.Count)];

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
                    PrizeName = prizeInfo.Name,
                    PrizeProvider = prizeInfo.Provider,
                    TicketName = ticket.Name,
                    TicketEmail = ticket.Email,
                    TicketPhone = ticket.Phone,
                    AccountPhone = account.Phone,
                    AccountName = account.Name,
                    DrawingDate = drawing.Date,
                    PrizesLeft = prizeInfos.Count - 1,
                    TicketsLeft = tickets.Count - 1
                };

                return Ok(winnerInfo);
            }
        }

        private async Task<Tuple<int, int>> GetPrizesAndTicketsLeft(Entities db, int drawingId)
        {
            var prizes = await(from p in db.Prizes
                               where p.DrawingId == drawingId
                               select new
                               {
                                   Quantity = p.Quantity - p.Winners.Count()
                               }).ToListAsync();

            var prizesLeft = 0;

            foreach (var prize in prizes)
            {
                for (int i = 0; i < prize.Quantity; i++)
                    prizesLeft++;
            }

            var ticketsLeft = await(from t in db.Tickets
                                    where t.DrawingId == drawingId && t.Winners.Count == 0
                                    select t).CountAsync();

            return new Tuple<int, int>(prizesLeft, ticketsLeft);
        }
    }
}
