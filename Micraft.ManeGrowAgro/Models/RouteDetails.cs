using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("RouteDetails")]
    public class RouteDetails
    {
        [Key]
        public int ID { get; set; }

        public int? RouteMainID { get; set; }

        public string FromRoute { get; set; }

        public string ToRoute { get; set; }

        public string VendorName { get; set; }

        public int? VendorID { get; set; }

        public int? KM { get; set; }

        public string Mode { get; set; } 

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public string RateType { get; set; }

        public int Unit { get; set; }

        public decimal Rate { get; set; }


        public decimal? WTFrom1 { get; set; }

        public decimal? WTTo1 { get; set; }

        public decimal? Rate1 { get; set; }

        public decimal? WTFrom2 { get; set; }

        public decimal? WTTo2 { get; set; }

        public decimal? Rate2 { get; set; }

        public decimal? WTFrom3 { get; set; }

        public decimal? WTTo3 { get; set; }

        public decimal? Rate3 { get; set; }

        public string VehicleType { get; set; }

        public string Mile { get; set; } 

    }
}