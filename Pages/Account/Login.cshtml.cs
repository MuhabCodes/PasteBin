using System;
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
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace PasteBin.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        
        private readonly LoginValidator _validator;
        
        [BindProperty]
        public InputModel Input { get; set; } = new();
        
        public LoginModel(ILogger<LoginModel> logger, PasswordHasher<ApplicationUser> hasher)
        {
            _logger = logger;
            _validator = new(hasher);
            
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

        public class LoginValidator: AbstractValidator<ApplicationUser>
        {
            public LoginValidator(PasswordHasher<ApplicationUser> hasher)
            {
                RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid e-mail Address.");
                RuleFor(x => x).Custom((val, context) =>
                {
                    string jsonPath = Path.Combine(Locations.UsersLocation, $"{val.Email}.json");
                    if (!System.IO.File.Exists(jsonPath))
                    {
                        context.AddFailure("This e-mail is not registered.");
                        return;
                    }
                    string jsonString = System.IO.File.ReadAllText(jsonPath);
                    ApplicationUser user = JsonSerializer.Deserialize<ApplicationUser>(jsonString);

                    if (hasher.VerifyHashedPassword(user,user.Password,val.Password) != PasswordVerificationResult.Success)
                    {
                        context.AddFailure("The e-mail or password that you have entered is incorrect.");
                    }
                });
            }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = new();
            user.Email = Input.Email;
            user.Password = Input.Password;
            var result2 = _validator.Validate(user);
            if (!result2.IsValid)
            {
                foreach (var message in result2.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.ErrorMessage);
                }
                _logger.LogInformation(LogEvents.LoginFailed, "Invalid login attempt for {User} at {UtcNow}", user.Email, DateTime.UtcNow);
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

            _logger.LogInformation(LogEvents.LoginSuccess, "User {Email} has logged in at {UtcNow}", user.Email, DateTime.UtcNow);

            return RedirectToPage("/Index");
        }
    }
}