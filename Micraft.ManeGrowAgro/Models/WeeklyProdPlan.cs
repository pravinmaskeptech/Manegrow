using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("WeeklyProdPlan")]
    public class WeeklyProdPlan
    {
        [Key] 
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        //public int Week { get; set; } 
        public string RoomNo { get; set; }

        public int? D1 { get; set; }

        public int? D2 { get; set; }

        public int? D3 { get; set; }

        public int? D4 { get; set; }

        public int? D5 { get; set; }

        public int? D6 { get; set; }

        public int? D7 { get; set; }

        public int? D8 { get; set; }

        public int? D9 { get; set; }

        public int? D10 { get; set; }

        public int? D11 { get; set; }

        public int? D12 { get; set; }

        public int? D13 { get; set; }

        public int? D14 { get; set; }

        public int? D15 { get; set; }

        public int? D16 { get; set; }

        public int? D17 { get; set; }

        public int? D18 { get; set; }

        public int? D19 { get; set; }

        public int? D20 { get; set; }

        public int? D21 { get; set; }

        public int? D22 { get; set; }

        public int? D23 { get; set; }

        public int? D24 { get; set; }

        public int? D25 { get; set; }

        public int? D26 { get; set; }

        public int? D27 { get; set; }

        public int? D28 { get; set; }

        public int? D29 { get; set; }

        public int? D30 { get; set; }

        public int? D31 { get; set; }

        [NotMapped]
        public int? Day1 { get; set; }

        [NotMapped]
        public int? Day2 { get; set; }

        [NotMapped]
        public int? Day3 { get; set; }

        [NotMapped]
        public int? Day4 { get; set; }

        [NotMapped]
        public int? Day5 { get; set; }

        [NotMapped]
        public int? Day6 { get; set; }

        [NotMapped]
        public int? Day7 { get; set; }

        public int? Total { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public DateTime FromDate { get; set; }

        //public DateTime? ToDate { get; set; }


        [NotMapped]
        public string FromDate { get; set; }

        [NotMapped]
        public string ToDate { get; set; }




    }
}