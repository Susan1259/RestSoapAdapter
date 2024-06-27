using System.ComponentModel.DataAnnotations;

namespace MyApplication.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<UserRole> UserRoles { get; set; }
    }

    }
}
