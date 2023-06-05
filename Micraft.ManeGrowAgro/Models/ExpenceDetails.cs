using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ExpenceDetails")]
    public class ExpenceDetails
    {
        [Key]
        public int ID { get; set; }

        public int? DispatchID { get; set; }

        public int? ExpMainID { get; set; }

        public string ExpType { get; set; }

        public decimal? Amount { get; set; }

        public string ImageName { get; set; }


        public string CashMemo { get; set; }

        public string ReceiptNo { get; set; }

        public DateTime? ReceiptDate { get; set; }

     [NotMapped]
        public string Name { get; set; }


        [NotMapped]
        public string strDate { get; set; } 
    }
}