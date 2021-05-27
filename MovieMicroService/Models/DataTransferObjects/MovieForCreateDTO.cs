using System;

namespace MovieMicroService.Models.DataTransferObjects
{
    public class MovieForCreateDTO
    {
        public string Name { get; set; }
        public DateTime StartMovie { get; set; }
        public DateTime EndMovie { get; set; }
        public string Description { get; set; }
        public int CountRows { get; set; }
        public int CountSites { get; set; }
    }
}
