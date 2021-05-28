namespace TicketMicroService.Models.DataTransferObjects
{
    public class MessageDetailsForCreateDTO
    {
        public int StatusCode { get; set; }
        public TicketForReadDTO Ticket { get; set; }
    }
}
