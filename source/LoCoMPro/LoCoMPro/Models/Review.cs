using LoCoMPro.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model for the Review Entity that represents the relation
    /// when a user reviews a register
    /// </summary>
    
    [PrimaryKey(nameof(ReviewerId), nameof(ContributorId), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Review
    {
        /// <summary>
        /// Navigation attribute for user that review the register.
        /// </summary>
        public required User Reviewer { get; set; }

        /// <summary>
        /// Navigation attribute for user that submitted the register.
        /// </summary>
        public required Register ReviewedRegister { get; set; }

        /// <summary>
        /// Navigation attribute for user that submitted the register.
        /// </summary>
        [ForeignKey(nameof(ReviewerId))]
        public string ReviewerId { get; set; }

        /// <summary>
        /// Navigation attribute for user that submitted the register.
        /// </summary>
        [ForeignKey(nameof(ContributorId))]
        public string ContributorId { get; set; }

        /// <summary>
        /// Navigation attribute for product that the register refers to.
        /// </summary>
        [ForeignKey(nameof(ProductName))]
        public string ProductName { get; set; }

        /// <summary>
        /// Navigation attribute for the store that the register refers to.
        /// </summary>
        [ForeignKey(nameof(StoreName))]
        public string StoreName { get; set; }

        /// <summary>
        /// Date when the register was submitted.
        /// </summary>
        [ForeignKey(nameof(SubmitionDate))]
        public DateTime SubmitionDate { get; set; }

        /// <summary>
        /// Name of the canton where the store is.
        /// </summary>
        public required string CantonName { get; set; }

        /// <summary>
        /// Name of the province where the store is.
        /// </summary>
        public required string ProvinceName { get; set; }

        /// <summary>
        /// Date when the register was reviewed.
        /// </summary>
        public required DateTime ReviewDate { get; set; }

        /// <summary>
        /// Value of the review given by the user.
        /// </summary>
        [DefaultValue(0)]
        public float ReviewValue { get; set; }

    }
}
