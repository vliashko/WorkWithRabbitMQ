using System;

namespace ReservationMicroService.Models.DataTransferObjects
{
    public class ReservationModelForSearchDTO
    {
        public string Telephone { get; set; }
        public DateTime DateTime { get; set; }
    }
}
