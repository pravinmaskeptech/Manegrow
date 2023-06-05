using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("OrderDetails")]   
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int MasterOrderId { get; set; } 

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int? Qty { get; set; }

        public string UOM { get; set; }

        public decimal? Weight { get; set; }
        public DateTime? Date { get; set; }

        public string PackagingRemark { get; set; }

        public string OrderStatus { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }  

        public decimal? Rate { get; set; }

        public string RateIN { get; set; }

        public decimal? Amount { get; set; } 


        [NotMapped]
        public string CustomerName { get; set; }

        public int? DispatchID { get; set; }

        public int? OrderStage { get; set; }
        public int? OldQty { get; set; }

        public int? MergeOrderNo { get; set; }

        public int? RejectedQty { get; set; }

        public string PODName { get; set; }

        public decimal? TodaysRate { get; set; }

        public DateTime? TodaysRateDate { get; set; }
        public string TodaysRateUpdatedBy { get; set; } 

    }
}