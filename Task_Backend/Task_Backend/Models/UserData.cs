using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Task_Backend.Models
{
    public class UserData
    {
        [Key]
        public string? UserDataId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(30, ErrorMessage = "Last Name cannot exceed 30 characters.")]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; } = null!;
    }
}
