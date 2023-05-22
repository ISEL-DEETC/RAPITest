using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _sender;
        private readonly ILogger _logger = Log.Logger;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            try
            {
                if (email == null)
                {
                    _logger.Warning("No email specified");
                    return RedirectToPage("/Index");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.Warning($"Unable to load user with email '{email}'");
                    return NotFound($"Unable to load user with email '{email}'.");
                }

                Email = email;
                // Once you add a real email sender, you should remove this code that lets you confirm the account
                DisplayConfirmAccountLink = true;
                if (DisplayConfirmAccountLink)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    EmailConfirmationUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                }

                return Page();
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message);
                return Page();
            }
        }
    }
}
