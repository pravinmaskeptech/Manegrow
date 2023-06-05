using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    public partial class TrackingDetails
    {
        [Key] 
        public int ID { get; set; }

        public int OrderNo { get; set; }

        public DateTime? Date { get; set; }


        public string StatusTime { get; set; } 
        public string Location { get; set; }

        public string Status { get; set; }

        public string Action { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } 

        public string Remark { get; set; } 
    }

}