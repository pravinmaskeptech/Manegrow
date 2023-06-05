using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("DriverMaster")]
    public class DriverMaster
    {
        [Key]
        public int Id { get; set; }

        public int? VendorID { get; set; } 
        public string DriverName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string MobileNo { get; set; }
        public string Reference { get; set; }
        public string ReferenceContact { get; set; }
        public string AdharNo { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }



    }
}