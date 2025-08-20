using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("DiscountCode")]
    public class DiscountCode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Column("discount_percentage", TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        [Required]
        [Column("max_uses")]
        public int MaxUses { get; set; }

        [Column("uses_count")]
        public int UsesCount { get; set; } = 0;

        [Column("min_order_value", TypeName = "decimal(10,2)")]
        public decimal? MinOrderValue { get; set; }

        [Required]
        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Required]
        [Column("end_date")]
        public DateOnly EndDate { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
