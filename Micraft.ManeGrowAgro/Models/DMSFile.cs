

namespace Micraft.ManeGrowAgro.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DMSFile
    {
        public int ID { get; set; }

        [StringLength(100)]
        public string Modulename { get; set; }

       
        public int? ModuleID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string AttachPath { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(30)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; } 

        [NotMapped]
        public int DmsFileId { get; set; }
        
        public string FileDetails { get; set; }

        public string DocumentNo { get; set; }

        [NotMapped]
        public int FormID { get; set; }

        public int? AssigneeID { get; set; }


        [StringLength(30)]
        public string UpdatedBy { get; set; }

        public string ModuleNo { get; set; }


    }
}
