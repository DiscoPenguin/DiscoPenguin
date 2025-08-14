using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTOs.Requests
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pasword is required")]
        public string Password { get; set; } = string.Empty;
    }

}