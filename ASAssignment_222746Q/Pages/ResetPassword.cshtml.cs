using ASAssignment_222746Q.Model;
using ASAssignment_222746Q.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASAssignment_222746Q.Pages
{
	public class ResetPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AuthDbContext _context;
		private readonly ILogger<ResetPasswordModel> _logger;

		[BindProperty]
		public ResetPassword RPModel { get; set; }

		public ResetPasswordModel(UserManager<ApplicationUser> userManager, AuthDbContext context, ILogger<ResetPasswordModel> logger)
		{
			_userManager = userManager;
			_context = context;
			_logger = logger;
			RPModel = new ResetPassword();
		}

		public void OnGet(string email, string resetToken)
		{
			RPModel.Email = email;
			RPModel.ResetToken = resetToken;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (string.IsNullOrEmpty(RPModel.ResetToken) || string.IsNullOrEmpty(RPModel.Email))
			{
				return RedirectToPage("/Error");
			}

			var user = _context.Users.FirstOrDefault(x => x.Email == RPModel.Email);
			if (user == null)
			{
				return RedirectToPage("/Error");
			}
			var result = await _userManager.ResetPasswordAsync(user, RPModel.ResetToken, RPModel.Password);
			if (result.Succeeded)
			{
				var userLog = _context.Users.ToList().Where(x => x.Email == RPModel.Email).FirstOrDefault();

				var auditLogResetPassword = new AuditLog
				{
					UserId = userLog.Id,
					Timestamp = DateTime.Now,
					ActionsTaken = "Password Reset",
					Information = $"{RPModel.Email} - Password Reset Successful"
				};

				_context.AuditLogger.Add(auditLogResetPassword);
				await _context.SaveChangesAsync();
				return RedirectToPage("/ResetPasswordConfirmation");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page();
			}
		}
	}
}
