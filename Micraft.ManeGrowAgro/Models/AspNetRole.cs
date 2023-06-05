using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace Micraft.ManeGrowAgro.Models 
{
    public class AspNetRole
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Discriminator { get; set; }

        [NotMapped]
        public string IdView { get; set; }

        [NotMapped]
        public string NameView { get; set; } 


    }
}