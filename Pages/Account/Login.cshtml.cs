using System;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using PasteBin.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace PasteBin.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        
        [BindProperty]
        public InputModel Input { get; set; } = new();
        
        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
            
            if (!Directory.Exists(Locations.UsersLocation))
            {
                Directory.CreateDirectory(Locations.UsersLocation);
            }
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            
        }
        
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser? user = AuthenticateUser(Input.Email, Input.Password);
                
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    
                    return Page();
                }

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("Password", user.Password),
                };
                
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                AuthenticationProperties authenticationProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow,
                    RedirectUri = "/Index"
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authenticationProperties);

                _logger.LogInformation("User {Email} has logged in at {UtcNow}",user.Email, DateTime.UtcNow);

                return RedirectToPage("/Index");
            }
            return Page();
        }

        private ApplicationUser? AuthenticateUser(string email, string password)
        {
            string jsonFile = Path.Combine(Locations.UsersLocation,$"{email}.json");

            if (!System.IO.File.Exists(jsonFile))   return null;
            string jsonString = System.IO.File.ReadAllText(jsonFile);
            ApplicationUser? User = JsonSerializer.Deserialize<ApplicationUser>(jsonString);

            if (User == null || User.Password != Hashing.hash(password,User.Email))
            {
                
                return null;
            } 

            return User;
        }
    }
}