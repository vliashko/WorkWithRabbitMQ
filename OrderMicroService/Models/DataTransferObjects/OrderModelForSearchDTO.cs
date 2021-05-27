using System;

namespace OrderMicroService.Models.DataTransferObjects
{
    public class OrderModelForSearchDTO
    {
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
    }
}
