using System;

namespace TicketMicroService.Models.DataTransferObjects
{
    public class TicketModelForSearchDTO
    {
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
    }
}
