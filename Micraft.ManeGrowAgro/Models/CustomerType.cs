using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("CustType")]
    public class CustomerType
    {
        public int? ID { get; set; }
        public string Type { get; set; }

        public bool? IsActive { get; set; }
        public string Prefix { get; set; }
        public int StartCode { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}