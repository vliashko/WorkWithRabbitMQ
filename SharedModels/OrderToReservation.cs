using System;

namespace SharedModels
{
    public class OrderToReservation
    {
        public TypeOperation Type { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
    }
}
