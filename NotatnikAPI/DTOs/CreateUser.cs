using System.ComponentModel.DataAnnotations;

namespace NotatnikAPI.DTOs
{
    public class CreateUser
    {
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
