using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{

    [Table("BoxTypeMaster")]
    public class BoxTypeMaster
    {
        public int Id { get; set; }

        public string BoxType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}