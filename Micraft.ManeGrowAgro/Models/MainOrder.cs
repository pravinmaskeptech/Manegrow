using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("MainOrder")]
    public class MainOrder
    {
        [Key]
        
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        public int? CustomerId { get; set; }

        public string CustomerName { get; set; } 
        public string Location { get; set; }

        public string CustomerCode { get; set; }

        public int CategoryId { get; set; }
        public string CustomerShortCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsApprove { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Remark { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }
        public string ExpectedDeliveryTime { get; set; }




        [NotMapped]
        public int OrderNo { get; set; }


        [NotMapped]

        public int TotalBoxes { get; set; }


        [NotMapped]

        public decimal TotalWT { get; set; }



        public decimal? OldReceiptAmount { get; set; }
        public decimal? AdjustAmount { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? Amount { get; set; }

        public decimal? FinalAmount { get; set; }

        public Boolean? IsPODUpload { get; set; } 


    }

}