using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("Address")]
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
    }
}
