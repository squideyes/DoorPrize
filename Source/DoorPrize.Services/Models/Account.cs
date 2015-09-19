using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorPrize.Services.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Index("IX_Account_Name", IsUnique = true)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Contest> Contests { get; set; }
    }
}