using System;

namespace TicketMicroService.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public DateTime StartMovie { get; set; }
        public int CountRows { get; set; }
        public int CountSites { get; set; }
    }
}
