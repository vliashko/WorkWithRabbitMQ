using System;
using System.Collections.Generic;

namespace TicketMicroService.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }
        public Guid PaymentCode { get; set; }

        public Reservation()
        {
            Places = new List<Place>();
        }
    }
}
