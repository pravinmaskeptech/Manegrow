using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("TableNumbring")]
    public class TableNumbring
    {
        [Key] 
        public int ID { get; set; }

        public string TableName { get; set; }

        public string Prefix { get; set; }

        public int? SerialNo { get; set; }

    }
}