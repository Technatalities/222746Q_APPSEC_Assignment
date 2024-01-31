using ASAssignment_222746Q.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace ASAssignment_222746Q.Pages
{
	[Authorize]
	public class IndexModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly AuthDbContext _context;
		private readonly ILogger<IndexModel> _logger;

		// User Details
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string NRIC { get; set; }
		public string Email { get; set; }
		public DateTime DOB { get; set; }
		public string ResumeFilePath { get; set; }
		public string ResumeFileName { get; set; }	
		public string WhoAmI { get; set; }

		public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext context, ILogger<IndexModel> logger)
		{
			_userManager = userManager;
			this.signInManager = signInManager;
			_context = context;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			var UserInfo = await _userManager.GetUserAsync(User);

			if (UserInfo == null)
			{
				 RedirectToPage("Login");
			}

			var MinutesSinceChange = (DateTime.Now - UserInfo.LastChangeDate).Minutes;

			if (MinutesSinceChange > 30)
			{
				RedirectToPage("ChangePassword");
			}

			if (UserInfo.SessionId != HttpContext.Session.GetString("SessionId"))
			{
				await _userManager.UpdateSecurityStampAsync(UserInfo);

				var auditLogLogout = new AuditLog
				{
					Timestamp = DateTime.Now,
					ActionsTaken = "Session Expiry",
					Information = $"{UserInfo.Email}"
				};

				auditLogLogout.UserId = UserInfo.Id;
				auditLogLogout.Information += " - Session Timeout";

				_context.AuditLogger.Add(auditLogLogout);

				await _context.SaveChangesAsync();

				await _userManager.UpdateAsync(UserInfo);
				await signInManager.SignOutAsync();
				RedirectToPage("Login");
			}

			var Encryption = DataProtectionProvider.Create("EncryptData");
			var Encryptor = Encryption.CreateProtector("ASAssignment_222746Q");

			if (UserInfo != null) 
			{
				FirstName = UserInfo.FirstName;
				LastName = UserInfo.LastName;
				Gender = UserInfo.Gender;
				if (UserInfo.NRIC != null)
				{
					NRIC = Encryptor.Unprotect(UserInfo.NRIC);
				}
				Email = UserInfo.Email;
				DOB = UserInfo.DOB;
				ResumeFilePath = UserInfo.ResumeFilePath;
				ResumeFileName = UserInfo.ResumeFileName;
				WhoAmI = UserInfo.WhoAmI;
			}
		}
	}
}