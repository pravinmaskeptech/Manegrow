using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("VendorQuotationMain")]
    public class VendorQuotationMain
    {
        [Key]
        public int ID { get; set; }

        public string MainRouteName{ get; set; } 

        public DateTime? QuotationDate { get; set; }

        public bool? GSTApplicable { get; set; }

        public decimal? FSCharges { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? MainRouteID { get; set; } 

    }
}