using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("RoomMaster")]
    public class RoomMaster
    {
        [Key]
        public int Id { get; set; }

        public string RoomNo { get; set; }


    }
}