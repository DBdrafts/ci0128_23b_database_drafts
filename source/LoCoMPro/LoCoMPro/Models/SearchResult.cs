using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;


namespace LoCoMPro.Models
{
    [Keyless]
    public class SearchResult
    {
        public DateTime SubmitionDate { get; set; }
        public string ContributorId { get; set; }
        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public string CantonName { get; set; }
        public string ProvinciaName { get; set; }
        public string Comment { get; set; }
        public uint NumCorrections { get; set; }
        public float Price { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public Point? Geolocation { get; set; }
        public float? Distance { get; set; }
    }
}
