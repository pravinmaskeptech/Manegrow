using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("TransMode")]
    public class TransMode
    {
        public int ID { get; set; }
        public string Mode { get; set; }
    }
}