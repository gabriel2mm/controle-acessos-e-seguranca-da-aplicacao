using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Middleware.Domain.Models
{
    public class Role : IdentityRole<Guid>
    {
        [Column("Organização")]
        public Organization Organization { get; set; }
    }
}
