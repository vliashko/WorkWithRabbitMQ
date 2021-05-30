using System;

namespace SharedModels
{
    public class OrderToReservationShared
    {
        public TypeOperation Type { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public Guid PaymentCode { get; set; }
    }
}
