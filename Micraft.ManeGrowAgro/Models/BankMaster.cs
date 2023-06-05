using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("BankMaster")]
    public class BankMaster
    {
        public int ID { get; set; }
        [Required, Display(Name = "Account Holder")]
        public int LinkID { get; set; }
        [Required, Display(Name = "Account Name")]
        public string AccName { get; set; }
        [Required, Display(Name = "Account Number")]
        public string AccNumber { get; set; }
        [Required, Display(Name = "Bank Name")]
        public string BranchName { get; set; }
        [Required, Display(Name = "City")]
        public string BranchCity { get; set; }
        [Required, Display(Name = "State")]
        public string BranchState { get; set; }
        [Required, Display(Name = "MICR Code")]
        public string MICRCode { get; set; }
        [Required, Display(Name = "IFSC COde")]
        public string IFSCCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }
}