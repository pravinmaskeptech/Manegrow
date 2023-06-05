using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("VehicleTypes")]
    public class VehicleTypes
    {
      public int ID { get; set; }

        [Display(Name= "Vehicle Type")]
      public string VehicleType { get; set; }
      public string CreatedBy { get; set; }
      public DateTime? CreatedDate { get; set; }
    }
}