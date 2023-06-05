using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    [Table("EmployeeMaster")]
    public class EmployeeMaster
    {
        public int ID { get; set; }

        [Required,Display(Name ="Name")]
        public string Name { get; set; }

        public string Address { get; set; }

        [Required, Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required, Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required, Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [ Display(Name = "Adhar Number")]
        public string AdharNumber { get; set; }

        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }

        [Required, Display(Name = "Reporting Person")]
        public int? ReportingToID { get; set; }

        [Required, Display(Name = "Designation")]
        public int DesigID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmpCode { get; set; }
        public string PhotoUpload { get; set; }
        public string Area { get; set; }

        public string City { get; set; } 
        public string Pincode { get; set; }

        public string UserRole { get; set; }


        [NotMapped]
        public string RoleID { get; set; }

        [NotMapped]

        public string RoleName { get; set; }

        [NotMapped]
        public string IdView { get; set; }

        [NotMapped]
        public string NameView { get; set; }


        public Boolean? CustomerOrder { get; set; }
        public Boolean? Harvesting { get; set; }
        public Boolean? Packing { get; set; }
        public Boolean? Dispatch { get; set; }
        public Boolean? PODUpdation { get; set; }
        public Boolean? Collection { get; set; }
        public Boolean? RateUpdation { get; set; }
        public Boolean? LocationDetails { get; set; }
        public Boolean? AllMaster { get; set; }
        public Boolean? Reports { get; set; }
        public Boolean? ResetPassword { get; set; }

    }   
}