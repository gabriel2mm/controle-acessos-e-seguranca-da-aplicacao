using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Middleware.Domain.Models
{
    [Table("Usuario")]
    public class User : IdentityUser<Guid>
    {
        [Required]
        [Column("Nome")]
        [StringLength(maximumLength: 50)]
        public String Name { get; set; }

        [Required]
        [Column("Usuario")]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        public String Login { get; set; }

        [ForeignKey("OrganizationID")]
        [Column("Organização")]
        public Organization Organization {get; set;}
        
        [Required]
        [Column("Ativo")]
        [DefaultValue(true)]
        public bool Active { get; set; }

        [NotMapped]
        public String Password { get; set; }
    }
}
