using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Employee_Managment.DTO
{
    public class RegistrationViewModel : IdentityUser
    {
        [Required]
        public string? EmpCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }

}
