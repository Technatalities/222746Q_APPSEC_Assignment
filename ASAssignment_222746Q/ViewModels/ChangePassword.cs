using System.ComponentModel.DataAnnotations;

namespace ASAssignment_222746Q.ViewModels
{
	public class ChangePassword
	{
		[Required]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		[MinLength(12, ErrorMessage = "Password must be a minimum length of 12 characters")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Please confirm password")]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "Password and confirmation password does not match")]
		public string ConfirmNewPassword { get; set; }
	}
}
