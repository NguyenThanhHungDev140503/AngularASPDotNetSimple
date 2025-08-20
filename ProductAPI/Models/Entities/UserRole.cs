using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("UserRole")]
    public class UserRole
    {
        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [Column("role_id")]
        public Guid RoleId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
    }
}
