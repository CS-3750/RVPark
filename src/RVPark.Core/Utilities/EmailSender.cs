using Microsoft.AspNetCore.Identity.UI.Services;

namespace RVPark.Core.Utilities
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Log the details or simply do nothing
            Console.WriteLine($"FAKE EMAIL: To={email}, Subject={subject}, Message={htmlMessage}");
            return Task.CompletedTask;
        }
    }
}