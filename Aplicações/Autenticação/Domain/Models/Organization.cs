using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Middleware.Domain.Models
{
    [Table("Organização")]
    public class Organization
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column("Nome")]
        public String Name { get; set; }
        [Column("Modules")]
        public ICollection<Module> Modules { get; set; }

        [Column("Ativo")]
        [DefaultValue(true)]
        public bool Active { get; set; }
    }
}
