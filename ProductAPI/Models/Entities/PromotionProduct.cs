using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("PromotionProduct")]
    public class PromotionProduct
    {
        [Required]
        [Column("promotion_id")]
        public Guid PromotionId { get; set; }

        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        // Navigation properties
        [ForeignKey("PromotionId")]
        public virtual Promotion Promotion { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
