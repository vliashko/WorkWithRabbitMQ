using System;
using System.Collections.Generic;

namespace ReservationMicroService.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}
