using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("City")]
    public class City
    {
        public int ID { get; set; }
        [Required(ErrorMessage ="State field is required!")]
        public string State { get; set; }

        [Required(ErrorMessage ="City field is required!")]
        public string CityName { get; set; }

        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}