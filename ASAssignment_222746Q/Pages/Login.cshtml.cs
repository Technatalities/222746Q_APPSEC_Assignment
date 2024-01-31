using System.Net;
using System.Text.Json;
using ASAssignment_222746Q.Model;
using ASAssignment_222746Q.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASAssignment_222746Q.Pages
{
	public class LoginModel : PageModel
	{
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly AuthDbContext _context;

		private readonly UserManager<ApplicationUser> userManager;

		private readonly ILogger<LoginModel> _logger;

		public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext context, ILogger<LoginModel> logger)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			_context = context;
			_logger = logger;
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
				LModel.RememberMe, lockoutOnFailure: true);

				var userLog = _context.Users.ToList().Where(x => x.Email == LModel.Email).FirstOrDefault();

				if (userLog == null)
				{
					return RedirectToPage("Login");
				}

				var auditLogLogin = new AuditLog
				{
					Timestamp = DateTime.Now,
					ActionsTaken = "Login Attempt",
					Information = $"{LModel.Email}"
				};

				if (identityResult.Succeeded)
				{
					HttpContext.Session.Clear();

					var sessionId = Guid.NewGuid().ToString();

					HttpContext.Session.SetString("SessionId", sessionId);

					userLog.SessionId = sessionId;

					auditLogLogin.UserId = userLog.Id;

					auditLogLogin.Information += " - Login Successful";

					_context.AuditLogger.Add(auditLogLogin);
					await _context.SaveChangesAsync();

					return RedirectToPage("Index");

				}
				if (identityResult.IsLockedOut)
				{
					auditLogLogin.UserId = userLog.Id;
					auditLogLogin.Information += " - Account Locked Out";

					_context.AuditLogger.Add(auditLogLogin);
					await _context.SaveChangesAsync();

					ModelState.AddModelError("", "Account is locked out. Please try again later.");
				}
				else
				{
					auditLogLogin.UserId = userLog.Id;
					auditLogLogin.Information += " - Login Failed";
					_context.AuditLogger.Add(auditLogLogin);
					await _context.SaveChangesAsync();
					ModelState.AddModelError("", "Username or Password incorrect");
				}
			}
			return Page();
		}
	}
}
