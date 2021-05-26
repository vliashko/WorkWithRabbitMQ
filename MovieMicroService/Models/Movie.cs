using System;

namespace MovieMicroService.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Duration { get; set; }
        public string Description { get; set; }
        public int[,] Places { get; set; }
    }
}
