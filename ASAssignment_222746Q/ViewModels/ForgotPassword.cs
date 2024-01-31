using System.ComponentModel.DataAnnotations;

namespace ASAssignment_222746Q.ViewModels
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
