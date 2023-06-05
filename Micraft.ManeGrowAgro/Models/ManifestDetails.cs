using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ManifestDetails")]
    public class ManifestDetails
    {
        [Key]
        public int ManifestDetailsID { get; set; }

        public int ManifestMainID { get; set; }

        public string DeliveryChallanNo { get; set; }

        public int? DispatchID { get; set; }

        public int? OrderNo { get; set; }

        public int? CustomerID { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string Location { get; set; }

        public string ProductName { get; set; }

        public decimal? OrderWeight { get; set; }

        public int? DispatchQty { get; set; }

        public int? BoxQty { get; set; }

        public decimal? BoxInKG { get; set; }

        public string InvoiceNo { get; set; }

    }
}