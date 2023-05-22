using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public LoginWithRecoveryCodeModel(SignInManager<ApplicationUser> signInManager, ILogger logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            try
            {
                // Ensure the user has gone through the username & password screen first
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new InvalidOperationException($"Unable to load two-factor authentication user.");
                }

                ReturnUrl = returnUrl;

                return Page();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warning("Unable to load two-factor authentication user.");
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new InvalidOperationException($"Unable to load two-factor authentication user.");
                }

                var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

                if (result.Succeeded)
                {
                    _logger.Information("User with ID '{UserId}' logged in with a recovery code.", user.Id);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
                if (result.IsLockedOut)
                {
                    _logger.Warning("User with ID '{UserId}' account locked out.", user.Id);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    _logger.Warning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                    ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                    return Page();
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warning("Unable to load two-factor authentication user");
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Page();
            }
        }
    }
}
