
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Employee_Managment.DTO
{
    public class LoginViewModel : IdentityUser
    {
        [Required]
        public string? EmpCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }

}
