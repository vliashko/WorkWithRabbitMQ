using System;
using System.Collections.Generic;

namespace TicketMicroService.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }

        public Ticket()
        {
            Places = new List<Place>();
        }
    }
}
