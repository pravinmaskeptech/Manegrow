using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("Zones")]
    public class Zones
    {
        public int ID { get; set; }

        public string CityFrom { get; set; }

        public string CityTo { get; set; }

        public string Zone { get; set; }

        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}