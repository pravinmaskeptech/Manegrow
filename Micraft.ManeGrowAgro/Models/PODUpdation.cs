using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("PODUpdation")]
    public class PODUpdation
    {
        [Key]
        public int? ID { get; set; }
        public DateTime Date { get; set; }
        public int? OrderNo { get; set; }
        public string UOM { get; set; }
        public decimal? Weight { get; set; }
        public int? CustID { get; set; }
        public string ProductName { get; set; }
        public int? Qty { get; set; }
        public int? ReceivedQty { get; set; }
        public int? RejectedQty { get; set; }
        public int? DamageReasonID { get; set; }

        public string DamageReason { get; set; }

        public string InvoiceNo { get; set; } 

    


        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; } 

        
        public string PODName { get; set; }

        public string CustomerName { get; set; } 

    }
}