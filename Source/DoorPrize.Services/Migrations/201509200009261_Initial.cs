namespace DoorPrize.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Phone = c.String(nullable: false, maxLength: 10, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Phone, unique: true, name: "IX_Account_Name");
            
            CreateTable(
                "dbo.Drawings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .Index(t => new { t.AccountId, t.Date }, unique: true, name: "IX_Drawing_AccountId_Date");
            
            CreateTable(
                "dbo.Prizes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DrawingId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Quantity = c.Int(nullable: false),
                        Provider = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Drawings", t => t.DrawingId)
                .Index(t => new { t.DrawingId, t.Name }, unique: true, name: "IX_Ticket_DrawingId_Name");
            
            CreateTable(
                "dbo.Winners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrizeId = c.Int(nullable: false),
                        TicketId = c.Int(nullable: false),
                        WonOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId)
                .ForeignKey("dbo.Prizes", t => t.PrizeId)
                .Index(t => new { t.PrizeId, t.TicketId }, unique: true, name: "IX_Winner_PrizeId_TicketId");
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DrawingId = c.Int(nullable: false),
                        Phone = c.String(nullable: false, maxLength: 10, unicode: false),
                        Email = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Drawings", t => t.DrawingId)
                .Index(t => new { t.DrawingId, t.Phone }, unique: true, name: "IX_Ticket_DrawingId_Phone");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Drawings", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Tickets", "DrawingId", "dbo.Drawings");
            DropForeignKey("dbo.Prizes", "DrawingId", "dbo.Drawings");
            DropForeignKey("dbo.Winners", "PrizeId", "dbo.Prizes");
            DropForeignKey("dbo.Winners", "TicketId", "dbo.Tickets");
            DropIndex("dbo.Tickets", "IX_Ticket_DrawingId_Phone");
            DropIndex("dbo.Winners", "IX_Winner_PrizeId_TicketId");
            DropIndex("dbo.Prizes", "IX_Ticket_DrawingId_Name");
            DropIndex("dbo.Drawings", "IX_Drawing_AccountId_Date");
            DropIndex("dbo.Accounts", "IX_Account_Name");
            DropTable("dbo.Tickets");
            DropTable("dbo.Winners");
            DropTable("dbo.Prizes");
            DropTable("dbo.Drawings");
            DropTable("dbo.Accounts");
        }
    }
}
