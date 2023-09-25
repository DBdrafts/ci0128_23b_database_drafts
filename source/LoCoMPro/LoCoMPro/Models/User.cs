using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(UserName))]
    public class User
    {
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Canton? Location { get; set; }
    }
}
