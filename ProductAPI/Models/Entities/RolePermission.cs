using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models.Entities
{
    [Table("RolePermission")]
    public class RolePermission
    {
        [Required]
        [Column("role_id")]
        public Guid RoleId { get; set; }

        [Required]
        [Column("permission_id")]
        public Guid PermissionId { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;
    }
}
