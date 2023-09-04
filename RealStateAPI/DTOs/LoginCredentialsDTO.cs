using System.ComponentModel.DataAnnotations;

namespace RealStateAPI.DTOs
{
    public class LoginCredentialsDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
