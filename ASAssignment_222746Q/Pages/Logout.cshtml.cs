using ASAssignment_222746Q.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASAssignment_222746Q.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
		private UserManager<ApplicationUser> userManager { get; }
		private readonly AuthDbContext _context;
		public LogoutModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext context)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			_context = context;
		}
		public void OnGet()
		{
		}
		public async Task<IActionResult> OnPostLogoutAsync()
		{
			var user = await userManager.GetUserAsync(User);

			if (user == null)
			{
				return RedirectToPage("Login");
			}
			user.SessionId = Guid.NewGuid().ToString();

			var auditLogLogout = new AuditLog
			{
				Timestamp = DateTime.Now,
				ActionsTaken = "Logout Attempt",
				Information = $"{user.Email}"
			};

			auditLogLogout.UserId = user.Id;
			auditLogLogout.Information += " - Logout Successful";

			_context.AuditLogger.Add(auditLogLogout);

			await userManager.UpdateAsync(user);
			await _context.SaveChangesAsync();

			await signInManager.SignOutAsync();
			return RedirectToPage("Login");
		}

		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
