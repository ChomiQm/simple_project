using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Task_Backend.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required] 
        public string LastName { get; set;} = null!;

        [Required]
        public string City { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime? LastActive { get; set; }

    }
}