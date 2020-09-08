using Middleware.Domain.Enumerators;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Middleware.Domain.Models
{
    [Table("Request")]
    public class Request
    {
        [Key]
        public Guid Id { get; set; }

        [Column("Users")]
        public User User { get; set; }

        [Column("Status")]
        public Status Status { get; set; }

        [Column("Types")]
        [Required]
        public Types Type { get; set; }

        [Column("Equipament")]
        [Required]
        public String Equipament { get; set; }

        [Column("Description")]
        [Required]
        public String Description { get; set; }

        [DefaultValue(false)]
        [Column("DPTOPayment")]
        public bool IsDptoPayment { get; set; }

        [DefaultValue(false)]
        [Column("Approval")]
        public bool Approval { get; set; }

        [Column("DescriptionDeclineApproval")]
        public String DescriptionDeclineApproval { get; set; }

        [Column("DescriptionSupport")]
        public String DescriptionsSupport { get; set; }

        [Column("TechnicianDescription")]
        public String TechnicianDescription { get; set; }

        [Column("Scheduling")]
        public DateTime Scheduling { get; set; }
    }
}
