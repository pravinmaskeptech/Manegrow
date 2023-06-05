using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("DispatchTransactions")] 
    public class DispatchTransactions
    {
        [Key] 
        public int ID { get; set; }
      
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public DateTime? Date { get; set; }
        public string VendorName { get; set; }
        public int VendorID { get; set; } 
        public decimal LoadINKG { get; set; }
        public int NoOfBoxes { get; set; }
        public decimal Amount { get; set; }
        public int DispatchID { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string RateType { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TotalKM { get; set; }
        public string RouteName { get; set; }        

    }
}