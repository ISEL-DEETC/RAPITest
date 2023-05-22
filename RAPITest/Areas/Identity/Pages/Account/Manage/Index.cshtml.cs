using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger = Log.Logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            try
            {
                var userName = await _userManager.GetUserNameAsync(user);
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

                Username = userName;

                Input = new InputModel
                {
                    PhoneNumber = phoneNumber
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                await LoadAsync(user);
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

                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return Page();
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return RedirectToPage();
                    }
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return NotFound("Due to Error");
            }
        }
    }
}
