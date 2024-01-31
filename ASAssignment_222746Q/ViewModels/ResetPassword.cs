using System.ComponentModel.DataAnnotations;

namespace ASAssignment_222746Q.ViewModels
{
	public class ResetPassword
	{
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[DataType(DataType.Text)]
		public string ResetToken { get; set; }

		[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		[MinLength(12, ErrorMessage = "Password must be a minimum length of 12 characters")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Please confirm password")]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
		public string ConfirmPassword { get; set; }
	}
}
