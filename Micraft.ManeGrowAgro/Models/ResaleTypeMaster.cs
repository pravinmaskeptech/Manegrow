using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("tblResaleTypes")]
    public class ResaleTypeMaster
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "ResaleType")]
        public string ResaleType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}