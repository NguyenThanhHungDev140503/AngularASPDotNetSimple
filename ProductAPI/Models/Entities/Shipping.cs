using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductAPI.Models.Enums;

namespace ProductAPI.Models.Entities
{
    [Table("Shipping")]
    public class Shipping
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Required]
        [Column("address_id")]
        public Guid AddressId { get; set; }

        [Required]
        public ShippingStatus Status { get; set; } = ShippingStatus.Pending;

        [Column("shipper_id")]
        public Guid? ShipperId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; } = null!;

        [ForeignKey("ShipperId")]
        public virtual User? Shipper { get; set; }
    }
}
