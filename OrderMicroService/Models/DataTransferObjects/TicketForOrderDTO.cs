using System;

namespace OrderMicroService.Models.DataTransferObjects
{
    public class TicketForOrderDTO
    {
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
        public int TicketCounts { get; set; }
    }
}
