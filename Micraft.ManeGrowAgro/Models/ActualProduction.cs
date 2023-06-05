using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ActualProduction")]
    public class ActualProduction
    {
        [Key]
        public int Id { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public string RoomNo { get; set; }

        public decimal? Total { get; set; }

        public decimal? D1 { get; set; }

        public decimal? D2 { get; set; }

        public decimal? D3 { get; set; }

        public decimal? D4 { get; set; }

        public decimal? D5 { get; set; }

        public decimal? D6 { get; set; }

        public decimal? D7 { get; set; }

        public decimal? D8 { get; set; }

        public decimal? D9 { get; set; }

        public decimal? D10 { get; set; }

        public decimal? D11 { get; set; }

        public decimal? D12 { get; set; }

        public decimal? D13 { get; set; }

        public decimal? D14 { get; set; }

        public decimal? D15 { get; set; }

        public decimal? D16 { get; set; }

        public decimal? D17 { get; set; }

        public decimal? D18 { get; set; }

        public decimal? D19 { get; set; }

        public decimal? D20 { get; set; }

        public decimal? D21 { get; set; }

        public decimal? D22 { get; set; }

        public decimal? D23 { get; set; }

        public decimal? D24 { get; set; }

        public decimal? D25 { get; set; }

        public decimal? D26 { get; set; }

        public decimal? D27 { get; set; }

        public decimal? D28 { get; set; }

        public decimal? D29 { get; set; }

        public decimal? D30 { get; set; }

        public decimal? D31 { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }


        [NotMapped]
        public string CreatedDateS { get; set; }
        [NotMapped]
        public string UpdatedDateS { get; set; } 
    }
}