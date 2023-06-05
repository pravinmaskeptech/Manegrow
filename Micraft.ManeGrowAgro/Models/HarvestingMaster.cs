using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("HarvestingMaster")]
    public class HarvestingMaster
    {
        public long ID { get; set; }

        public int? RoomNumber { get; set; }

        public DateTime? ProjectionDate { get; set; }

        public decimal? YearlyProjection { get; set; }

        public decimal? MonthlyProjection { get; set; }

        public decimal? DailyPrediction { get; set; }

        public decimal? ActualQuentity { get; set; }

        public string ProjectionFailuarReason { get; set; }

        public bool? IsProjection { get; set; }
    }
}