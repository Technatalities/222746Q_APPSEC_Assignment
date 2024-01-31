using System.Threading.Tasks;
using ASAssignment_222746Q.Model;
using ASAssignment_222746Q.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASAssignment_222746Q.Pages
{
	public class ChangePasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly AuthDbContext _context;

		public string PasswordAgeError { get; set; } = string.Empty;

		[BindProperty]
		public ChangePassword CPModel { get; set; }

		public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return RedirectToPage("/Login");
			} 

			if (user.SessionId != HttpContext.Session.GetString("SessionId"))
			{
				await _userManager.UpdateSecurityStampAsync(user);

				var auditLogLogout = new AuditLog
				{
					Timestamp = DateTime.Now,
					ActionsTaken = "Session Expiry",
					Information = $"{user.Email}"
				};

				auditLogLogout.UserId = user.Id;
				auditLogLogout.Information += " - Session Timeout";

				_context.AuditLogger.Add(auditLogLogout);

				await _context.SaveChangesAsync();

				await _userManager.UpdateAsync(user);
				await _signInManager.SignOutAsync();
				return RedirectToPage("/Login");
			}


			var MinutesSinceChange = (DateTime.Now - user.LastChangeDate).Minutes;
			if (MinutesSinceChange < 5)
			{
				ModelState.AddModelError(string.Empty, "Your password must be at least 5 minutes old to be changed.");
				return Page();
			}

			if (IsSameAsOldPassword(user, CPModel.NewPassword))
			{
				ModelState.AddModelError(string.Empty, "Your New Password cannot be the same as your last 2 passwords.");
				return Page();
			}

			var result = await _userManager.ChangePasswordAsync(user, CPModel.OldPassword, CPModel.NewPassword);

			if (result.Succeeded)
			{
				var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, CPModel.NewPassword);
				user.OldPassword2 = user.OldPassword1; 
				user.OldPassword1 = newPasswordHash;
				user.LastChangeDate = DateTime.Now;

				var auditLogChangePassword = new AuditLog
				{
					UserId = user.Id,
					Timestamp = DateTime.Now,
					ActionsTaken = "Change Password",
					Information = $"{user.Email} - Password Change Successful"
				};

				_context.AuditLogger.Add(auditLogChangePassword);
				await _context.SaveChangesAsync();

				await _userManager.UpdateAsync(user);
				await _signInManager.SignInAsync(user, isPersistent: false);

				return RedirectToPage("/ChangePasswordConfirmation");
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
		private bool IsSameAsOldPassword(ApplicationUser user, string newPassword)
		{
			return _userManager.PasswordHasher.VerifyHashedPassword(user, user.OldPassword1, newPassword) == PasswordVerificationResult.Success ||
				   _userManager.PasswordHasher.VerifyHashedPassword(user, user.OldPassword2, newPassword) == PasswordVerificationResult.Success;
		}

	}
}
