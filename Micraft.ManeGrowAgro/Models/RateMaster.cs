using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("RateMaster")]
    public class RateMaster1 
    {
        [Key]
        public int ID { get; set; }
      
        public DateTime? Date { get; set; }

        [Display(Name = "Product ID")]

        public int ProductID { get; set; }

        [Display(Name = "Product Name")]

        public string ProductName { get; set; }

        public decimal? Weight { get; set; }

        public string UOM { get; set; }

        [Display(Name = "Today Rate ")]

        public decimal? TodayRate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; }   
        public DateTime? UpdatedDate { get; set; }   


    }
}