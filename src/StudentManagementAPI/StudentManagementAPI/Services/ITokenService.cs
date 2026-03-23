using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}