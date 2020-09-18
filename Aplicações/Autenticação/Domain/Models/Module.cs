using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Middleware.Domain.Models
{
    [Table("Modulo")]
    public class Module
    {
        [Key]
        public Guid Id { get; set; }
        [Column("Nome")]
        public String Name { get; set; }
        [Column("Permissao")]
        public String Permission { get; set; }
        [Column("Organizacao")]
        public Organization Organization { get; set; }

        [Column("Ativo")]
        [DefaultValue(true)]
        public bool Active { get; set; }
    }
}
