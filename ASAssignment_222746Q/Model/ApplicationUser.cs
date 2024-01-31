using Microsoft.AspNetCore.Identity;

namespace ASAssignment_222746Q.Model
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string NRIC { get; set; }
		public DateTime DOB { get; set; }
		public string ResumeFilePath { get; set; }
		public string ResumeFileName { get; set; }
		public string WhoAmI { get; set; }
		public string SessionId { get; set; } = string.Empty;
		public string OldPassword1 { get; set; } = string.Empty;
		public string OldPassword2 { get; set; } = string.Empty;
		public DateTime LastChangeDate { get; set; }
	}
}
