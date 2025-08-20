using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("CartItem")]
    public class CartItem
    {
        [Required]
        [Column("cart_id")]
        public Guid CartId { get; set; }

        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
