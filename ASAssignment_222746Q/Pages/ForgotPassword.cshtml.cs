using ASAssignment_222746Q.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ASAssignment_222746Q.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASAssignment_222746Q.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly EmailService _emailService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<ForgotPasswordModel> _logger;
		private readonly AuthDbContext _context;

		[BindProperty]
        public ForgotPassword FPModel { get; set; }

        public ForgotPasswordModel(EmailService emailService, UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordModel> logger, AuthDbContext context)
        {
			_emailService = emailService;
			_userManager = userManager;
			_logger = logger;
			_context = context;
		}
		public async Task<IActionResult> OnPostAsync()
		{
			var user = _context.Users.ToList().Where(x => x.Email == FPModel.Email).FirstOrDefault();
			if (user == null)
			{
				_logger.LogInformation("User not found");
				return RedirectToPage("/ForgotPasswordConfirmation");
			}
			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

			var resetLink = Url.PageLink("ResetPassword", values: new { email = user.Email, resetToken });

			await _emailService.SendEmailAsync(FPModel.Email, "Password Reset", $"Click here to reset your password: {resetLink}");

			return RedirectToPage("/ForgotPasswordConfirmation");
		}

		private string GenerateUniqueToken()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
