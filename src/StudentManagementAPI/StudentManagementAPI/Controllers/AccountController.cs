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

        // 2. Add ITokenService to the constructor parameters
        public AccountController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ITokenService tokenService) // Inject it here
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _tokenService = tokenService; // 3. Assign the value
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate the token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Create the confirmation link
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                // Send the "email" (it will appear in your Visual Studio Console)
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
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

            // This internal Identity method validates the token and updates the DB
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.IsEmailConfirmed = true; // Update your custom flag
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

            // Generate a new token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create the confirmation link
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            // Send the "email" to the console
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking here: {confirmationLink}");

            return Ok("Confirmation email resent. Please check the console.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials.");

            if (!user.EmailConfirmed)
                return Unauthorized("Please confirm your email before logging in.");

            // Generate the JWT token
            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                Email = user.Email,
                Token = token
            });
        }
    }
}