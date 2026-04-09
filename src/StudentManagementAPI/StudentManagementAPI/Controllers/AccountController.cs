using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "The email already exists. Please login instead." });
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleToAssign = "Student";
                if (model.Role == "Employer") roleToAssign = "Employer";
                if (model.Role == "Admin") roleToAssign = "Admin";
                await _userManager.AddToRoleAsync(user, roleToAssign);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email!, "Confirm your email",
                    $"Please confirm your account by clicking here: {confirmationLink}");

                return Ok("Registration successful. Check the console for your verification link.");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest("Invalid link.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.IsEmailConfirmed = true; 
                await _userManager.UpdateAsync(user);
                return Ok("Email confirmed successfully! You can now log in.");
            }

            return BadRequest("Email confirmation failed.");
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found.");

            if (user.EmailConfirmed) return BadRequest("Email is already confirmed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email",
                $"Please confirm your account by clicking here: {confirmationLink}");

            return Ok("Confirmation email resent. Please check the console.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { message = "Credentials do not match our records." });

            if (!user.EmailConfirmed)
                return Unauthorized(new { message = "Please verify your email before signing in." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new
            {
                Email = user.Email,
                Token = token
            });
        }
    }
}