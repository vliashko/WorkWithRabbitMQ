using System.Collections.Generic;

namespace ReservationMicroService.Models.DataTransferObjects
{
    public class ReservationForUpdateDTO
    {
        public IEnumerable<Place> Places { get; set; }
        public bool IsFooled { get; set; }
    }
}
