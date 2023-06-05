using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{ [Table("CollectionMain")]
    public class CollectionMain
    {
        [Key]
        public int ID { get; set; }

        public int? CustID { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PaymentMode { get; set; }

        public decimal? PayAmount { get; set; }

        public string PaymentDetails { get; set; }
        public string TransactionID { get; set; }
        public DateTime? TransactionDate { get; set; }
       






        [NotMapped]
        public string InvoiceNo { get; set; }
        [NotMapped]
        public DateTime? InvoiceDate { get; set; }
        [NotMapped]
        public int? CollectionID { get; set; }
        [NotMapped]
        public decimal? OldReceiptAmount { get; set; }
        [NotMapped]
        public decimal? ReceivableAmount { get; set; }
        [NotMapped]
        public decimal? RecdAmount { get; set; }
        [NotMapped]
        public decimal? AdjustAmount { get; set; }

        [NotMapped]
        public int? Mainid { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}