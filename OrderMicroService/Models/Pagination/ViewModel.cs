using System.Collections.Generic;

namespace OrderMicroService.Models.Pagination
{
    public class ViewModel<T>
    {
        public IEnumerable<T> Objects { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
