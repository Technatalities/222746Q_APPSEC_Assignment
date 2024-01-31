using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using ASAssignment_222746Q.Model;
using ASAssignment_222746Q.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASAssignment_222746Q.Pages
{
	public class RegisterModel : PageModel
	{
		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }
		private readonly IConfiguration configuration;

		[BindProperty]
		public Register RModel { get; set; }
		public RegisterModel(UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IConfiguration configuration)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.configuration = configuration;
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ValidateGoogleCaptcha())
			{
				ModelState.AddModelError("", "Captcha validation failed.");
				return Page();
			}

			ValidateAlphabetical("FirstName", RModel.FirstName, "First Name");
			ValidateAlphabetical("LastName", RModel.LastName, "Last Name");
			ValidateNRIC(RModel.NRIC);
			ValidateEmail(RModel.Email);
			ValidateDOB(RModel.DOB);
			ValidateFileExtension(RModel.ResumeFile);

			if (ModelState.IsValid)
			{
				var Encryption = DataProtectionProvider.Create("EncryptData");
				var Encryptor = Encryption.CreateProtector("ASAssignment_222746Q");

				var ResumeFileName = RModel.ResumeFile.FileName;
				var ResumeFilePath = UploadFile(RModel.ResumeFile);

				var sessionId = Guid.NewGuid().ToString();

				var user = new ApplicationUser()
				{
					UserName = RModel.Email,
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					Gender = RModel.Gender,
					NRIC = Encryptor.Protect(RModel.NRIC),
					Email = RModel.Email,
					DOB = RModel.DOB,
					ResumeFilePath = ResumeFilePath,
					ResumeFileName = ResumeFileName,
					WhoAmI = WebUtility.HtmlEncode(RModel.WhoAmI),
					SessionId = sessionId,
				};

				user.OldPassword1 = userManager.PasswordHasher.HashPassword(user, RModel.Password);

				var result = await userManager.CreateAsync(user, RModel.Password);

				if (result.Succeeded)
				{
					return RedirectToPage("Login");
				}
				foreach (var error in result.Errors)
				{
					if (error.Code == "DuplicateUserName")
					{
						ModelState.AddModelError("", "Email is already taken");
					}
					else
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}
			return Page();
		}

		private string UploadFile(IFormFile ResumeFile)
		{
			if (ResumeFile != null && ResumeFile.Length > 0)
			{
				var ResumeFileName = Guid.NewGuid().ToString() + "_" + ResumeFile.FileName;
				var ResumeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", ResumeFileName);

				using (var stream = new FileStream(ResumeFilePath, FileMode.Create))
				{
					ResumeFile.CopyTo(stream);
				}

				return ResumeFileName;
			}
			return null;
		}

		private void ValidateAlphabetical(string Name, string Value, string FieldName)
		{
			if (!string.IsNullOrEmpty(Value) && !Regex.IsMatch(Value, @"^[a-zA-Z]+$"))
			{
				ModelState.AddModelError(Name, $"{FieldName} must be alphabets only");
			}
		}

		private void ValidateNRIC(string NRIC)
		{
			if (!string.IsNullOrEmpty(NRIC) && !Regex.IsMatch(NRIC, @"^[STFG]\d{7}[A-Z]$"))
			{
				ModelState.AddModelError("NRIC", "NRIC must be in the format of S1234567A");
			}
		}

		private void ValidateEmail(string Email)
		{
			if (!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
			{
				ModelState.AddModelError("Email", "Email is invalid");
			}
		}

		private void ValidateDOB(DateTime DateOfBirth)
		{
			if (DateOfBirth > DateTime.Today)
			{
				ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
			}
		}

		private void ValidateFileExtension(IFormFile File)
		{
			if (File != null)
			{
				var FileType = Path.GetExtension(File.FileName).ToLower();
				if (FileType != ".pdf" && FileType != ".docx")
				{
					ModelState.AddModelError("ResumeFile", "Resume must be in PDF or DOCX format");
				}
			}
		}

		public class RecaptchaObject
		{
			public bool success { get; set; }
		}

		public bool ValidateGoogleCaptcha()
		{
			var ReCaptchaSecretKey = configuration["ReCaptchaKey"];
			string Response = Request.Form["g-recaptcha-response"];
			string SecretKey = ReCaptchaSecretKey;
			bool Valid = false;

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + SecretKey + "&response=" + Response);

			try
			{
				using (WebResponse wResponse = req.GetResponse())
				{
					using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
					{
						string jsonResponse = readStream.ReadToEnd();

						RecaptchaObject data = JsonSerializer.Deserialize<RecaptchaObject>(jsonResponse);

						Valid = data.success;
					}
				}
				return Valid;
			}
			catch (WebException ex)
			{
				throw ex;
			}

		}

	}
}
