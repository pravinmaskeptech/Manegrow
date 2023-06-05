using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("VendorCityMaster")]
    public partial class VendorCityMaster
    {
        [Key] 
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string RouteName { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Boolean? BomApplicable { get; set; } 
    }
}
