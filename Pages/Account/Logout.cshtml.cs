using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        
        public IActionResult OnPostAsync()
        {
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                _httpContext.HttpContext.Response.Cookies.Delete("LoginCookie");
            }
            return RedirectToPage("/Index");
        }
    }
}