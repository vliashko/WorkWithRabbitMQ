namespace MovieMicroService.Models.DataTransferObjects
{
    public class MessageDetailsForCreateDTO
    {
        public int StatusCode { get; set; }
        public MovieForReadDTO Movie { get; set; }
    }
}
