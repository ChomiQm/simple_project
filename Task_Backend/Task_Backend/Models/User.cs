using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Task_Backend.Models
{
    public class User : IdentityUser
    {
        [DataType(DataType.DateTime)]
        public DateTime? LastActive { get; set; }

        public virtual UserData? UserData { get; set; }
    }
}