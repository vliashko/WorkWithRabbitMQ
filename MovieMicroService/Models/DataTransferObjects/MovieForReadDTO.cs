using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace MovieMicroService.Models.DataTransferObjects
{
    public class MovieForReadDTO
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime StartMovie { get; set; }
        public DateTime EndMovie { get; set; }
        public string Description { get; set; }
        public int CountRows { get; set; }
        public int CountSites { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}
