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
    /// Model for the Report Entity that represents the relation
    /// when a user reports a register
    /// </summary>

    [PrimaryKey(nameof(ReporterId), nameof(ContributorId), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Report
    {
        /// <summary>
        /// Navigation attribute for user that report the register.
        /// </summary>
        public required User Reporter { get; set; }

        /// <summary>
        /// Navigation attribute for user that submitted the register.
        /// </summary>
        public required Register ReportedRegister { get; set; }

        /// <summary>
        /// Navigation attribute for user that submitted the register.
        /// </summary>
        [ForeignKey(nameof(ReporterId))]
        public string? ReporterId { get; set; }

        /// <summary>
        /// Navigation attribute for user that submitted the register.
        /// </summary>
        [ForeignKey(nameof(ContributorId))]
        public string? ContributorId { get; set; }

        /// <summary>
        /// Navigation attribute for product that the register refers to.
        /// </summary>
        [ForeignKey(nameof(ProductName))]
        public string? ProductName { get; set; }

        /// <summary>
        /// Navigation attribute for the store that the register refers to.
        /// </summary>
        [ForeignKey(nameof(StoreName))]
        public string? StoreName { get; set; }

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
        /// Date when the register was reported.
        /// </summary>
        public required DateTime ReportDate { get; set; }

        /// <summary>
        /// Value of the report given by the user.
        /// </summary>
        [DefaultValue(0)]
        public int ReportState { get; set; }

    }
}
