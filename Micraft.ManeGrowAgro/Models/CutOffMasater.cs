using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("CutOffMaster")]
    public class CutOffMasater
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "City")]
        public string CityCutOff { get; set; }
        [Required]
        public string Mode { get; set; }
        [Required]
        public string Vendor { get; set; }
        [Required, Display(Name = "Packing")]
        public string PackingCutoff { get; set; }
        [Required, Display(Name = "Plant")]
        public string PlantCutoff { get; set; }
        [Required, Display(Name = "Dispatch")]
        public string DispatchCutoff { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}