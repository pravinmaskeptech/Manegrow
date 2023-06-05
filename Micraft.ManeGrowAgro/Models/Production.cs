using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("Production")]
    public class Production
    {
        [Key]
        public int Id { get; set; }

        public DateTime? Date { get; set; }
        public string RoomNo { get; set; }
        public string CaretType { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? ActualWeight { get; set; }
        public int? NoOfCaret { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}