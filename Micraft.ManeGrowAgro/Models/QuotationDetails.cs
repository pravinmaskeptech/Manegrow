using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    public class QuotationDetails
    {
        [Key] 
        public int DetailsID { get; set; }
        public int MainID { get; set; }
        public int ProdCategoryID { get; set; }
        public string ProdCategory { get; set; } 
        public string ProductName { get; set; }
        public int ProductID { get; set; }        
        public string UOM { get; set; }

        public decimal? WTFrom3 { get; set; }
        public decimal? WTTo3 { get; set; } 
        public decimal? Rate3 { get; set; }

        public decimal? WTFrom1 { get; set; }
        public decimal? WTTo1 { get; set; }
        public decimal? Rate1 { get; set; }


        public decimal? WTFrom2 { get; set; }
        public decimal? WTTo2 { get; set; }
        public decimal? Rate2 { get; set; }

        public decimal? AddWt { get; set; }
        public decimal? AddRate { get; set; }


        [NotMapped]
        public string QuotationName { get; set; }
        [NotMapped]
        public string QuotationDate { get; set; }

    }
}