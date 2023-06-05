using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("SubRouteMain")]  
    public class SubRouteMain 
    {
        public int ID { get; set; }

        public int MainRouteID { get; set; }
        public string SubRouteName { get; set; }
        public string MainRouteName { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}