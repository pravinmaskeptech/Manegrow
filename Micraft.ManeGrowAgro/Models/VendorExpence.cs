using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ExpenceMain")]
    public class VendorExpence
    {
        [Key]
        public int ID { get; set; }

        public int? DispatchID { get; set; }

        public int? FromKM { get; set; }

        public int? ToKM { get; set; }

        public int? TotalKM { get; set; }

        public decimal? TotalAmount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}