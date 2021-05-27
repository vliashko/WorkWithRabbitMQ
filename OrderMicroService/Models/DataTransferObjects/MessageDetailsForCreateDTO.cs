namespace OrderMicroService.Models.DataTransferObjects
{
    public class MessageDetailsForCreateDTO
    {
        public int StatusCode { get; set; }
        public OrderForReadDTO Order { get; set; }
    }
}
