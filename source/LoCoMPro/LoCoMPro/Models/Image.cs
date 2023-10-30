using LoCoMPro.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model for the Image entity, has a user, and a register asociated with it.
    /// Its intended to save the images of a product associated with a register
    /// </summary>
    [PrimaryKey(nameof(ImageId), nameof(ContributorId), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Image
    {

        /// <summary>
        /// Navigation atribute for user that submitted the image.
        /// </summary>
        [ForeignKey(nameof(ContributorId))]
        public required User Contributor { get; set; }


        /// <summary>
        /// Navigation atribute for the register that the image refers to.
        /// </summary>
        public Register? Register { get; set; }

        /// <summary>
        /// Image ID to identify a single image among several associated with a register.
        /// </summary>
        public required int ImageId { get; set; }

        /// <summary>
        /// Data when the register that the image refers to was submitted.
        /// </summary>
        public required DateTime SubmitionDate { get; set; }

        /// <summary>
        /// Image data converted to bytes.
        /// </summary>
        public byte[]? ImageData { get; set; }

        /// <summary>
        /// Image MIME type (PNG, JPG...)
        /// </summary>
        public string? ImageType { get; set; }

        /// <summary>
        /// Id of the user in the register that the image refers to.
        /// </summary>
        public required string ContributorId { get; set; }

        /// <summary>
        /// Name of the product in the register that the image refers to.
        /// </summary>
        public required string ProductName { get; set; }

        /// <summary>
        /// Name of the store in the register that the image refers to.
        /// </summary>
        public required string StoreName { get; set; }

        /// <summary>
        /// Name of the canton in the register that the image refers to.
        /// </summary>
        public required string CantonName { get; set; }

        /// <summary>
        /// Name of the province in the register that the image refers to.
        /// </summary>
        public required string ProvinceName { get; set; }

    }
}
