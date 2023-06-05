using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("RejectionSoldHistory")]
    public class RejectionSoldHistory
    {
        [Key]
        public int ID { get; set; }

        public int? RejectionID { get; set; }

        public int? OrderNo { get; set; }

        public int? CustID { get; set; }

        public string CustomerName { get; set; }

        public string ProductName { get; set; }

        public string ResaleType { get; set; }

        public string SoldCustomerName { get; set; }

        public decimal? Rate { get; set; }

        public int? Qty { get; set; }

        public decimal? TotalAmount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}