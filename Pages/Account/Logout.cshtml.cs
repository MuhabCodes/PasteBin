using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PasteBin.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        private readonly IHttpContextAccessor _httpContext;

        public LogoutModel(ILogger<LogoutModel> logger, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _httpContext = httpContext;
        }
        
        public void OnGet()
        {
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation(LogEvents.LogoutSuccess, "User {Email} has logged out at {UtcNow}", _httpContext.HttpContext.User.Identity.Name, DateTime.UtcNow);
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}