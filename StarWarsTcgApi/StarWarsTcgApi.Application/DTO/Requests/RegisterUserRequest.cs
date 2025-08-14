using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTOs.Requests
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Passowrd must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
        /*  
            [Required]
            [StringLength(100, MinimumLength = 6)]
            public string Password { get; set; } = string.Empty; //TODO: Password needs hashed and salted
        */
    }
}