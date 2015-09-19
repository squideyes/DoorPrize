using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorPrize.Services.Models
{
    public class Winner
    {
        public int Id { get; set; }

        [Required]
        [Index("IX_Winner_PrizeId_TicketId", IsUnique = true, Order = 1)]
        public int PrizeId { get; set; }

        [Required]
        [Index("IX_Winner_PrizeId_TicketId", IsUnique = true, Order = 2)]
        public int TicketId { get; set; }

        [Required]
        public DateTime WonOn { get; set; }

        public virtual Prize Prize { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}