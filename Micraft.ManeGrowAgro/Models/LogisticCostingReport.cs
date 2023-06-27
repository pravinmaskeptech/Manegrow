using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    public class LogisticCostingReport
    {
        public string VechicleNo { get; set; }
        public string ClientName { get; set; }
        public string Destination { get; set; }
        public decimal Box { get; set; }
        public decimal Weight { get; set; }
        public int CNG { get; set; }
        public decimal Ammount { get; set; }
        public string UOM { get; set; }
        public DateTime DispatchDate { get; set; }

    }
}