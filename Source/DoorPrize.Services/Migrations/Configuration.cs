namespace DoorPrize.Services.Migrations
{
    using Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Entities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Entities context)
        {
            context.Accounts.AddOrUpdate(
                a => a.Phone,
                new Account
                {
                    Name = "DuPont",
                    Phone = "5705078275"
                });
        }
    }
}
