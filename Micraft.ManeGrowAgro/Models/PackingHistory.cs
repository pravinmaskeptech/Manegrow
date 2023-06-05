using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("PackingHistory")] 
    public class PackingHistory
    {
        public int ID { get; set; }

        public int SalesOrderNo { get; set; } 

        public DateTime? OrderDate { get; set; }

        public int? CustomerID { get; set; }

        public string CustomerName { get; set; }
        public string ProductName { get; set; }

        public int? OldQty { get; set; }

        public int? NewQty { get; set; }

        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }
}