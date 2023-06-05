using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("SubRouteCustomers")]
    public class SubRouteCustomers
    {
        [Key]
        public int ID { get; set; }
        public int RouteDetailsID { get; set; }        
        public int RouteMainID { get; set; }    
        public int CustomerID { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}