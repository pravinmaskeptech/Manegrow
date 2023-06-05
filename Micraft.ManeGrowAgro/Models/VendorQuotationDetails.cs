using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    public class VendorQuotationDetails
    {
        [Key]
        public int DetailsID { get; set; }

        public int? MainID { get; set; }

        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public string UOM { get; set; }

        public decimal? WTFrom1 { get; set; }

        public decimal? WTTo1 { get; set; }

        public decimal? Rate1 { get; set; }

        public decimal? WTFrom2 { get; set; }

        public decimal? WTTo2 { get; set; }

        public decimal? Rate2 { get; set; }

        public decimal? WTFrom3 { get; set; }

        public decimal? WTTo3 { get; set; }

        public decimal? Rate3 { get; set; }

        public decimal? AddWt { get; set; }

        public decimal? AddRate { get; set; }


        public int? SubRouteID { get; set; }

        public string SubRoute { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }


        public int? VendorID { get; set; }

        public string VendorName { get; set; }



        [NotMapped]
        public DateTime? QuotationDate { get; set; }

        [NotMapped]
        public bool? GSTApplicable { get; set; }

        [NotMapped]
        public decimal? FSCharges { get; set; }

       

        

    }
}