using System;

namespace OrderMicroService.Models
{
    public class Order
    {
        public int Id { get; set; } 
        public string Telephone { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime PurchaseDateTime { get; set; }
        public int TotalTickets { get; set; }
        public Guid PaymentCode { get; set; }
    }
}
