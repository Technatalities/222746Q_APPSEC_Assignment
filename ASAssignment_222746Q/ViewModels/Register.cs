using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ASAssignment_222746Q.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter your first name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; } 

        [Required(ErrorMessage = "Please enter your last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter your NRIC")]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [DataType(DataType.Password)]
		[MinLength(12, ErrorMessage = "Password must be a minimum length of 12 characters")]
		public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your date of birth")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime DOB { get; set; }

        public IFormFile ResumeFile { get; set; }

        public string WhoAmI { get; set; }
    }
}
