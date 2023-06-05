using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("CaretMaster")]
    public class CaretMaster
    {
        [Key]
        public int Id { get; set; }

        public string CaretType { get; set; }

        [Display(Name = "Weight (Kg)")]
        public decimal? Weight { get; set; }
    }
}