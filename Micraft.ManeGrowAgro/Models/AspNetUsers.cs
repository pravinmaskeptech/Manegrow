using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Micraft.ManeGrowAgro.Models
{
    public class AspNetUsers
    {
   
        [Key]
        public string Id { get; set; }

        [Column]
        public string Email { get; set; }

        [Column]
        public bool EmailConfirmed { get; set; }

        [Column]
        public string PasswordHash { get; set; }

        [Column]
        public string SecurityStamp { get; set; }

        [Column]
        public string PhoneNumber { get; set; }

        [Column]
        public bool PhoneNumberConfirmed { get; set; }

        [Column]
        public bool TwoFactorEnabled { get; set; }

        [Column]
        public DateTime? LockoutEndDateUtc { get; set; }

        [Column]
        public bool LockoutEnabled { get; set; }

        [Column]
        public int AccessFailedCount { get; set; }

        [Column]
        public string UserName { get; set; }

        [Column]
        public decimal? UserId { get; set; }

        [Column]
        public string FullName { get; set; }

        [Column]
        public string Address { get; set; }

        [Column]
        public bool? IsActive { get; set; }

        [Column]
        public string RegistrationType { get; set; }

    }
}