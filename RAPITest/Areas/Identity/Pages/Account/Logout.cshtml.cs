using System;
using System.Collections.Generic;
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
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger = Log.Logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            try
            {

                await _signInManager.SignOutAsync();
                _logger.Information("User logged out.");
                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    return RedirectToPage();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Page();
            }
        }
    }
}
