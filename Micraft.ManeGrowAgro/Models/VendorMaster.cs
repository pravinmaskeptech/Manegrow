using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("VendorMaster")]
    public class VendorMaster
    {
        public int ID { get; set; }


        [Display(Name = "Vendor Type")]

        public string VendorType { get; set; }

        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Display(Name = "Pincode")]
        [Required]
        public string PinCode { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
       
        
        [Required, Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        [Required, Display(Name = "Email Address")]
        public string EmailId { get; set; }
        [Required]
        
        public string Address { get; set; }

        
        public int? QuotationID { get; set; }
      
        [Display(Name = "Adhar Number")]
        public string AadharNo { get; set; }
        
        [Display(Name = "Pan Number")]
        public string PanNo { get; set; }
       
        public string GSTNo { get; set; }
        
        public string VendorPhoto { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }
}