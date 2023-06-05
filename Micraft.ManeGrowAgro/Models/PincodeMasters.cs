using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("PincodeMasters")]
    public class PincodeMasters
    {
        public int ID { get; set; }

        public string Pincode { get; set; }

        public string City { get; set; }

        public string Zone { get; set; }

        public string State { get; set; }

        public string Area { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}