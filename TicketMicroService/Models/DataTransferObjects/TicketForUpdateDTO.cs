using System.Collections.Generic;

namespace TicketMicroService.Models.DataTransferObjects
{
    public class TicketForUpdateDTO
    {
        public IEnumerable<Place> Places { get; set; }
        public bool IsFooled { get; set; }
    }
}
