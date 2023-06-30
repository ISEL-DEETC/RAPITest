using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public PersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                return Page();
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message);
                return NotFound("Due to Error");
            }
        }
    }
}