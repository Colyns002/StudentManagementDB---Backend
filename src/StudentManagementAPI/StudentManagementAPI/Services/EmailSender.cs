using Microsoft.AspNetCore.Identity.UI.Services;

namespace StudentManagementAPI.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For development: We will log the email to the Console
            // In production: You would use SendGrid, Mailtrap, or Gmail SMTP
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"TO: {email}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine($"BODY: {htmlMessage}");
            Console.WriteLine("------------------------------------------------");

            return Task.CompletedTask;
        }
    }
}