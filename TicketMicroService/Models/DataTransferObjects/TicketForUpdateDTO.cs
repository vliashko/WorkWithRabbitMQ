using System;
using System.Collections.Generic;

namespace TicketMicroService.Models.DataTransferObjects
{
    public class TicketForUpdateDTO
    {
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}
