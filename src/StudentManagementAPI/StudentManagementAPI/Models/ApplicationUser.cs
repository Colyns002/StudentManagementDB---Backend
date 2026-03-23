using Microsoft.AspNetCore.Identity;

namespace StudentManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEmailConfirmed { get; set; } = false;
    }
}