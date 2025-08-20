using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("PaymentMethod")]
    public class PaymentMethod
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Provider { get; set; }

        [Column("fee_percent", TypeName = "decimal(5,2)")]
        public decimal FeePercent { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
