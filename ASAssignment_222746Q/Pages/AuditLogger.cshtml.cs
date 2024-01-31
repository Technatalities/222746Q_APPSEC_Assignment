using ASAssignment_222746Q.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASAssignment_222746Q.Pages
{
    [Authorize]
    public class AuditLoggerModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public IList<AuditLog> AuditLogEntries { get; set; }

        public AuditLoggerModel(AuthDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task OnGetAsync()
        {
			var UserInfo = await _userManager.GetUserAsync(User);

			if (UserInfo == null ||  UserInfo.SessionId != HttpContext.Session.GetString("SessionId"))
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

			var DaySinceChange = (DateTime.Now - UserInfo.LastChangeDate).Days;
			if (DaySinceChange > 90)
			{
				RedirectToPage("ChangePassword");
			}

			AuditLogEntries = await _context.AuditLogger.ToListAsync();
        }
    }
}
