using System.Collections.Generic;

namespace MovieMicroService.Models.DataTransferObjects
{
    public class MovieForUpdateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}
