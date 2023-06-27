using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Micraft.ManeGrowAgro.Models
{
    [Table("PackingDetails")]
    public class PackingDetails
    {

        [Key]
        public int ID { get; set; }
        public int OrderNo { get; set; }
        public decimal? BoxWeight { get; set; }
        public int NoOfBox { get; set; }
        public decimal? TotalWeight { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? MergeOrderNo { get; set; }  
        public string UOM { get; set; }
        public string BoxType { get; set; }

    }


}