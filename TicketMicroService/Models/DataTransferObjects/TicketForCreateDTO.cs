using System;
using System.Collections.Generic;

namespace ReservationMicroService.Models.DataTransferObjects
{
    public class ReservationForCreateDTO
    {
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}
