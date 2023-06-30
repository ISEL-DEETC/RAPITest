using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                RequirePassword = await _userManager.HasPasswordAsync(user);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Page();
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

                RequirePassword = await _userManager.HasPasswordAsync(user);
                if (RequirePassword)
                {
                    if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                    {
                        ModelState.AddModelError(string.Empty, "Incorrect password.");
                        return Page();
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                var userId = await _userManager.GetUserIdAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
                }

                await _signInManager.SignOutAsync();

                _logger.Information("User with ID '{UserId}' deleted themselves.", userId);

                return Redirect("~/");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Page();
            }
            
        }
    }
}
