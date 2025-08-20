using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductAPI.Models.Enums;

namespace ProductAPI.Models.Entities
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Required]
        [Column("payment_method_id")]
        public Guid PaymentMethodId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(255)]
        [Column("provider_txn_id")]
        public string? ProviderTxnId { get; set; }

        [Column("provider_fee", TypeName = "decimal(10,2)")]
        public decimal ProviderFee { get; set; } = 0;

        public string? Metadata { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("idempotency_key")]
        public Guid? IdempotencyKey { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
    }
}
