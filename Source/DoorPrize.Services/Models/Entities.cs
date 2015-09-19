using System.Data.Entity;

namespace DoorPrize.Services.Models
{
    public class Entities : DbContext
    {
        public Entities()
            : base("name=DoorPrize")
        {
            Database.SetInitializer<Entities>(null);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Prize> Prizes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Winner> Winners { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(e => e.Name).IsUnicode(false);
            modelBuilder.Entity<Account>().Property(e => e.Phone).IsUnicode(false);

            modelBuilder.Entity<Prize>().Property(e => e.Name).IsUnicode(false);
            modelBuilder.Entity<Prize>().Property(e => e.Provider).IsUnicode(false);

            modelBuilder.Entity<Ticket>().Property(e => e.Name).IsUnicode(false);
            modelBuilder.Entity<Ticket>().Property(e => e.Phone).IsUnicode(false);
            modelBuilder.Entity<Ticket>().Property(e => e.Email).IsUnicode(false);

            modelBuilder.Entity<Account>().HasMany(e => e.Contests).
                WithRequired(e => e.Account).WillCascadeOnDelete(false);

            modelBuilder.Entity<Contest>().HasMany(e => e.Prizes).
                WithRequired(e => e.Contest).WillCascadeOnDelete(false);

            modelBuilder.Entity<Contest>().HasMany(e => e.Tickets).
                WithRequired(e => e.Contest).WillCascadeOnDelete(false);

            modelBuilder.Entity<Prize>().HasMany(e => e.Winners).
                WithRequired(e => e.Prize).WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>().HasMany(e => e.Winners).
                WithRequired(e => e.Ticket).WillCascadeOnDelete(false);
        }
    }
}