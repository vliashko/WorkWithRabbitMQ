
namespace ReservationMicroService.Models.DataTransferObjects
{
    public class MessageDetailsForCreateDTO
    {
        public int StatusCode { get; set; }
        public ReservationForReadDTO Reservation { get; set; }
    }
}
