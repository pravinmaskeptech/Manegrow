using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{

    [Table("PODNotUploadedVendorList")]
    public class PODNotUploadedVendorList
    {
        [Key]
        public int CustID { get; set; }

       
        public string CustName { get; set; }

        public string Location { get; set; }

      
        public string VendorName { get; set; } 
    }
}