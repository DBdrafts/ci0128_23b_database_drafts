using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name), nameof(Provincia))]
    public class Canton
    {
        public required string Name { get; set; }
        public required Provincia Provincia { get; set; }
    }
}
