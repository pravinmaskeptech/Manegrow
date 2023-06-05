using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("SubRouteDetails")] 
    public partial class SubRouteDetails
    { 
        [Key]
        public int ID { get; set; }
      
        public int RouteMainID { get; set; }
        public string RouteMainName { get; set; }
        public int SubRouteMainID { get; set; } 
        public string SubRouteName { get; set; }
        public string SR1 { get; set; }
        public string SR2 { get; set; }
        public string SR3 { get; set; }
        public string SR4 { get; set; }
        public string SR5 { get; set; }
        public string SR6 { get; set; }
        public string SR7 { get; set; }
        public string SR8 { get; set; } 

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy  { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}