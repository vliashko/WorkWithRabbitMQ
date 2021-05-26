using System.Collections.Generic;

namespace TicketMicroService.Models.Pagination
{
    public class ViewModel<T>
    {
        public IEnumerable<T> Objects { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
