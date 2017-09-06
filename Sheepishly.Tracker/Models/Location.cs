using System;

namespace Sheepishly.Tracker.Models
{
    public class Location
    {
        public Guid SheepId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
