using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace middleware.Models
{
    [Table("Usuario")]
    public class User : IdentityUser
    {
        [Required]
        [Column("Nome")]
        [StringLength(maximumLength: 50)]
        public String Name { get; set; }

        [Required]
        [Column("Usuario")]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        public String Login { get; set; }

        [NotMapped]
        public String Password { get; set; }
    }
}
