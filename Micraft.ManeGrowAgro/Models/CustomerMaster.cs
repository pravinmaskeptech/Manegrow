using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("CustomerMaster")]
    public class CustomerMaster
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        public string CustName { get; set; }

        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        public string Address { get; set; }

        public string Area { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [Display(Name = "Pincode")]
        public string PinCode { get; set; }

        [Display(Name = "Adhar Number")]
        public string AdharNumber { get; set; }

        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [Required]
        [Display(Name = "Mobile Number ")]
        public string MobileNumber { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Adhar Upload")]
        public string AdharUpload { get; set; }
        [Display(Name = "Pan Upload")]
        public string PanUpload { get; set; }
        [Display(Name = "GST Number")]
        public string GSTNumber { get; set; }
        [Display(Name = "Profile Photo")]
        public string ProfilePhoto { get; set; }
        [Required]
        [Display(Name = "Customer Type")]

        public int? CustTypeID { get; set; }
        [Display(Name = "Sales Person")]
        public int? SalesPersonID { get; set; }

       
        public string Password { get; set; }
      
        public string Username { get; set; }

        [Required]
        [Display(Name = "Customer Code")]
        public string CustID { get; set; }

        public bool? IsApproved { get; set; }




        public string CreatedBy { get; set; } 

        public DateTime? CreatedDate { get; set; } 

        public string UpdatedBy { get; set; } 

        public DateTime? UpdatedDate { get; set; }

        public int? QuotationID { get; set; }

        public string RootName { get; set; }


        public Boolean? IsActive { get; set; } 
    }
}