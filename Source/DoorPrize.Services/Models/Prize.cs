using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorPrize.Services.Models
{
    public class Prize
    {
        public int Id { get; set; }

        [Required]
        [Index("IX_Ticket_ContestId_Name", IsUnique = true, Order = 1)]
        public int ContestId { get; set; }

        [Required]
        [StringLength(50)]
        [Index("IX_Ticket_ContestId_Name", IsUnique = true, Order = 2)]
        public string Name { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string Provider { get; set; }

        public virtual Contest Contest { get; set; }

        public ICollection<Winner> Winners { get; set; }
    }
}