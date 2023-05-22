using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account.Manage
{
    public class GenerateRecoveryCodesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public GenerateRecoveryCodesModel(
            UserManager<ApplicationUser> userManager,
            ILogger logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                if (!isTwoFactorEnabled)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return NotFound("Due to Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                var userId = await _userManager.GetUserIdAsync(user);
                if (!isTwoFactorEnabled)
                {
                    throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
                }

                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                RecoveryCodes = recoveryCodes.ToArray();

                _logger.Information("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
                StatusMessage = "You have generated new recovery codes.";
                return RedirectToPage("./ShowRecoveryCodes");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return NotFound("Due to Error");
            }
        }
    }
}