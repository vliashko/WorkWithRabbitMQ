using System;
using System.Collections.Generic;

namespace SharedModels
{
    public class TicketShared
    {
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<PlaceShared> Places { get; set; }
    }
}
