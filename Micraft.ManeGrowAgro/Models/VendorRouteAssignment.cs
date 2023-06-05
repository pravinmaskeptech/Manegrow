using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{

    [Table("VendorRouteAssignment")] 
    public class VendorRouteAssignment
    {
        [Key]
        public int ID { get; set; }

        public int CustomerID { get; set; }

        public int VendorID { get; set; }

        public string CustomerName { get; set; }
        public string VendorName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }  

    }
}