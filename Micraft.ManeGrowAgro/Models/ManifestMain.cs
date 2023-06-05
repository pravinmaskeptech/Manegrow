using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("ManifestMain")]
    public class ManifestMain
    {
        [Key]
        public int ManifestMainID { get; set; }

        public int DispatchID { get; set; }

        public string DeliveryChallanNo { get; set; }


        public string RouteName { get; set; }

        public int RouteID { get; set; }

        public string GroupName { get; set; }

        public DateTime Date { get; set; }

        public string Mode { get; set; }

        public string TransportName { get; set; }

        public string VehicleNo { get; set; }

        public string VehicleType { get; set; }

        public string DriverName { get; set; }

        public string DriverMobileNo { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }


        public decimal TotalOrderWeight { get; set; }

        public int  TotalDispatchQty { get; set; }

        public int TotalBoxQty { get; set; }

        public decimal TotalBoxinKG { get; set; } 

    }

}