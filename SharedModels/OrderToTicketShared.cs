using System;

namespace SharedModels
{
    public class OrderToTicketShared
    {
        public TypeOperation Type { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
        public Guid PaymentCode { get; set; }
    }
}
