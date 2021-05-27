namespace MovieMicroService.Models
{
    public class Place
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Site { get; set; }
        public bool IsBusy { get; set; }
        public int MovieId { get; set; }
    }
}
