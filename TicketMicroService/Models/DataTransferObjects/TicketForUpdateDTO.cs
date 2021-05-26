using System;
using System.Collections.Generic;

namespace TicketMicroService.Models.DataTransferObjects
{
    public class TicketForUpdateDTO
    {
        public IEnumerable<PlaceForUpdateDTO> Places { get; set; }
    }
}
