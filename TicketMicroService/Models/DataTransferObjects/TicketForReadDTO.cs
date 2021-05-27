using System;
using System.Collections.Generic;

namespace ReservationMicroService.Models.DataTransferObjects
{
    public class ReservationForReadDTO
    {
        public int Id { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }
        public bool IsFooled { get; set; }
    }
}
