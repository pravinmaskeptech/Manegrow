using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ProductMaster")]
    public class ProductMaster
    {
        public long ID { get; set; }

        public string Name { get; set; }
   
        [Display(Name = "Product Type")]
        public int? TypeID { get; set; }

        public string Size { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Rate { get; set; }

        public string Image { get; set; }
        public bool? IsActive { get; set; }

        [Display(Name = "Prod Uom")]
        public string ProdUom { get; set; }
        public string HSCCode { get; set; }
       
        [Display(Name = "Description")]
        public string Discription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}