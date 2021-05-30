using System;
using System.Collections.Generic;

namespace SharedModels
{
    public class TicketShared
    {
        public TypeOperation Type { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<PlaceShared> Places { get; set; }
    }
}
