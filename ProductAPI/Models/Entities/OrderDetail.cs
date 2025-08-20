using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Required]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column("price_at_purchase", TypeName = "decimal(10,2)")]
        public decimal PriceAtPurchase { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
