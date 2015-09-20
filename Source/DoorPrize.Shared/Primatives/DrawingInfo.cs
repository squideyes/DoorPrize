using System;

namespace DoorPrize.Shared
{
    public class DrawingInfo
    {
        public string AccountPhone { get; set; }
        public string AccountName { get; set; }
        public DateTime DrawingDate { get; set; }
        public int PrizesLeft { get; set; }
        public int TicketsLeft { get; set; }
    }
}