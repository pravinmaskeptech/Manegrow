using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("DispatchDetails")] 
    public partial class DispatchDetails
    {
        [Key]
        public int DispatchID { get; set; }

        public DateTime DispatchDate { get; set; }

        public string DriverName { get; set; }

        public int? Route { get; set; }  

        public string VendorName { get; set; }

        public int? VendorID { get; set; }  
        public string VehicalNo { get; set; }

        public int NoofBoxes { get; set; }

        public decimal TotalWeight { get; set; }


        public string VendorType { get; set; }   
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }


        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; } 
    }
}