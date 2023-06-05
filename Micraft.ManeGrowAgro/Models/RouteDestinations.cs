using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    public partial class RouteDestinations 
    {
        [Key] 
        public int DestID { get; set; }

        public int RouteID { get; set; } 

        public string Destinations { get; set; } 

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }  

    }
}