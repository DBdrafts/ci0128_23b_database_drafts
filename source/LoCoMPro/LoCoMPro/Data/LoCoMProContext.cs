using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoCoMPro.Models;

namespace LoCoMPro.Data
{
    public class LoCoMProContext : DbContext
    {
        public LoCoMProContext (DbContextOptions<LoCoMProContext> options)
            : base(options)
        {
        }

        public DbSet<LoCoMPro.Models.Product> Products { get; set; } = default!;
    }
}
