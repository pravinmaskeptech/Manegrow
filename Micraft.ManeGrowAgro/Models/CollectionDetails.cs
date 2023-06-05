using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{ [Table("CollectionDetails")]
    public class CollectionDetails
    {
        [Key]
        public int ID { get; set; }

        public int? CollectionID { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? Amount { get; set; }

        public decimal? OldReceiptAmount { get; set; }

        public decimal? ReceivableAmount { get; set; }

        public decimal? RecdAmount { get; set; }

        public decimal? AdjustAmount { get; set; }

        [NotMapped]
        public int? Mainid { get; set; }

    }
}