using System;
using System.Collections.Generic;

namespace ReservationMicroService.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }
        public bool IsFooled { get; set; }

        public Reservation()
        {
            Places = new List<Place>();
        }
    }
}
