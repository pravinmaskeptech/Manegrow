using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ManifestGroup")]
    public class ManifestGroup
    {
        [Key]
        public int ID { get; set; }
        public int RouteID { get; set; }
        public string GroupName { get; set; }
        public int CustomerID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public string RouteName { get; set; }

    }
}