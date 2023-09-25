using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name))]
    public class Store
    {
        public required string Name { get; set; }
        public required Canton Location { get; set; }

    }
}
