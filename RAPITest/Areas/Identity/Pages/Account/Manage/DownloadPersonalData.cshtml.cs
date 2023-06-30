using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using Serilog;
using RAPITest.Models;

namespace RAPITest.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger logger)
        {
            _userManager = userManager;
            _logger = logger;
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

                _logger.Information("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

                // Only include personal data for download
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
                return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return NotFound("Due to Exception");
            }
        }
    }
}
