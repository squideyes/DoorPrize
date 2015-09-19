using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorPrize.Services.Models
{
    public class Contest
    {
        public int Id { get; set; }

        [Required]
        [Index("IX_Contest_AccountId_Date", IsUnique = true, Order = 1)]
        public int AccountId { get; set; }

        [Required]
        [CustomValidation(typeof(Contest), "ValidateDate")]
        [Index("IX_Contest_AccountId_Date", IsUnique = true, Order = 2)]
        public DateTime Date { get; set; }

        public virtual Account Account { get; set; }

        public ICollection<Prize> Prizes { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public static ValidationResult ValidateDate(DateTime date, ValidationContext context)
        {
            if (date.TimeOfDay != TimeSpan.Zero)
            {
                return new ValidationResult(
                      "The \"Date\" field must be set to a date with no time-of-day component.");
            }

            return ValidationResult.Success;
        }
    }
}