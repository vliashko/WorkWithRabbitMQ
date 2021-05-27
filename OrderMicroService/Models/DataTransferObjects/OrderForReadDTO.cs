using System;

namespace OrderMicroService.Models.DataTransferObjects
{
    public class OrderForReadDTO
    {
        public int Id { get; set; }
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime PurchaseDateTime { get; set; }
        public int TotalTickets { get; set; }
    }
}
