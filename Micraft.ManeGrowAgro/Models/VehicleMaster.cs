using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("VehicleMaster")]
    public class VehicleMaster
    {
        public int ID { get; set; }
        [Required]
        [Display(Name ="Vendor Name")]
        public int  VendorID { get; set; } 
        [Required]
        [Display(Name ="Vehicle No")]
        public string VehicleNo { get; set; }
        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }
        [Required]            
        [Display(Name = "Insurance Validity")]         
        public DateTime InsuranceValidity { get; set; }
        [Required]
        [Display(Name = "IS PUC")]
        public string Puc { get; set; }
        [Required]
        [Display(Name = "IS RC Book")]
        public string RCBook { get; set; }
        [Required]
        [Display(Name = "RC Validity")]
        public DateTime RCValidity { get; set; }
        [Required]
        [Display(Name = "Vehicle Capacity")]
        public string VehicleCapacity { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public string Attachments { get; set; }
        [NotMapped]
        public int DmsFileID { get; set; }

        [NotMapped]
        public string DocumentNo { get; set; }

        [NotMapped]
        public string Name { get; set; }
    }
}