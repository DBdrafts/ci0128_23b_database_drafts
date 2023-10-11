using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model of the Category entity, qualifies different products.
    /// </summary>
    [PrimaryKey(nameof(CategoryName))]
    public class Category
    {
        /// <summary>
        /// Name of the category.
        /// </summary>
        public required string CategoryName { get; set; }

        /// <summary>
        /// Products wich the category qualify.
        /// </summary>
        public ICollection<Product>? Products { get; set; } = new List<Product>();
    }
}