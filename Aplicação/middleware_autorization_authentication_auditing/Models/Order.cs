using middleware.Enumerators;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace middleware.Models
{
    [Table("Order")]
    public class Order
    {
        public Guid Id { get; set; }

        [Column("Users")]
        public User User { get; set; }

        [Column("Description")]
        [Required]
        public String Description { get; set; }

        [Column("Queue")]
        public Queue Queue { get; set; }

        [Column("Request")]
        [Required]
        public Request Request { get; set; }
    }
}
