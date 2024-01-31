using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASAssignment_222746Q.Pages
{
	[Authorize]
	public class ChangePasswordConfirmationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
