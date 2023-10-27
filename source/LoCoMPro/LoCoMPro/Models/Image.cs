using LoCoMPro.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(ImageId), nameof(ContributorId), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Image
    {

        [ForeignKey(nameof(ContributorId))]
        public required User Contributor { get; set; }

       
        public Register Register { get; set; }


        public required int ImageId { get; set; }
        public required DateTime SubmitionDate { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        public required string ContributorId { get; set; }
        public required string ProductName { get; set; }

        public required string StoreName { get; set; }
        public required string CantonName { get; set; }
        public required string ProvinceName { get; set; }

    }
}


//// Foreign Keys to reference the Register
//public required Product Product { get; set; }
//public required User Contributor { get; set; }
//public required Store Store { get; set; }
//public required DateTime SubmitionDate { get; set; }

//public string? ContributorId { get; set; }

//public string? ProductName { get; set; }

//public string? StoreName { get; set; }

//public Register? Register { get; set; }