namespace ReservationMicroService.Models
{
    public class Place
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Site { get; set; }
        public int ReservationId { get; set; }
    }
}
