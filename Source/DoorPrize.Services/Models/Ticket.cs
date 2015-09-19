using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorPrize.Services.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [Index("IX_Ticket_ContestId_Phone", IsUnique = true, Order = 1)]
        public int ContestId { get; set; }

        [Required]
        [Index("IX_Ticket_ContestId_Phone", IsUnique = true, Order = 2)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual Contest Contest { get; set; }

        public ICollection<Winner> Winners { get; set; }
    }
}