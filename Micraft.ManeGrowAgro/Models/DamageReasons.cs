using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("DamageReasons")]
    public class DamageReasons
    {
        [Key]
        public int ID { get; set; }
        public string DamageReason { get; set; }
    }
}