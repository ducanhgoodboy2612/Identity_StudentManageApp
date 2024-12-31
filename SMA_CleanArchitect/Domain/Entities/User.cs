using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        public string? Gender { get; set; }

        public string? Description { get; set; }

        public int RoleId { get; set; }

        public int IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        public string Token { get; set; }
    }
}
