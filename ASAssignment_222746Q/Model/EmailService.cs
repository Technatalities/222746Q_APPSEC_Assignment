using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace ASAssignment_222746Q.Model
{
	public class EmailService
	{
		private readonly IConfiguration configuration;
		public EmailService(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var EmailMessage = new MimeMessage();
			var EmailAddress = configuration["EmailAddress"];
			var EmailPassword = configuration["EmailPassword"];

			EmailMessage.From.Add(new MailboxAddress("Ace Job Agency", EmailAddress));
			EmailMessage.To.Add(new MailboxAddress("", toEmail));
			EmailMessage.Subject = subject;

			EmailMessage.Body = new TextPart("plain")
			{
				Text = body
			};

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync("smtp.gmail.com", 587, false);
				await client.AuthenticateAsync(EmailAddress, EmailPassword);
				await client.SendAsync(EmailMessage);
				await client.DisconnectAsync(true);
			}
		}
	}
}
