using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using PasteBin.Models;
using FluentValidation;


namespace PasteBin.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        private readonly RegisterValidator _validator = new();

        private readonly PasswordHasher<ApplicationUser> _hasher;

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; } = new();

        public RegisterModel(ILogger<RegisterModel> logger, PasswordHasher<ApplicationUser> hasher)
        {
            _logger = logger;
            _hasher = hasher; 

            if (!Directory.Exists(Locations.UsersLocation))
            {
                Directory.CreateDirectory(Locations.UsersLocation);
            }
        }

        public class RegisterValidator: AbstractValidator<ApplicationUser>
        {
            public RegisterValidator()
            {
                RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid e-mail Address.");
                RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password must be atleast 8 characters long.");
                RuleFor(x => x.Email).Custom((val, context) =>
                {
                    if (System.IO.File.Exists(Path.Combine(Locations.UsersLocation, $"{val}.json")))
                    {
                        context.AddFailure("This e-mail is already in use.");
                    }
                });
            }
        }
        
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var result = _validator.Validate(ApplicationUser);

            if (!result.IsValid)
            {
                foreach (var message in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.ErrorMessage);
                }
                _logger.LogInformation(LogEvents.RegisterFailed, "Invalid register attempt at {UtcNow}", DateTime.UtcNow); 
                return Page();
            }
            ApplicationUser.Password = _hasher.HashPassword(ApplicationUser, ApplicationUser.Password);
            // ApplicationUser.Password = Hashing.hash(ApplicationUser.Password,ApplicationUser.Email);

            string jsonString = JsonSerializer.Serialize(ApplicationUser);
            
            string fileName = $"{ApplicationUser.Email}.json";            
            string userPath = Path.Combine(Locations.UsersLocation, fileName);

            System.IO.File.WriteAllText(userPath, jsonString);

            _logger.LogInformation(LogEvents.RegisterSuccess, "A new account has been created user {Email} at {UtcNow}", ApplicationUser.Email, DateTime.UtcNow); 
            return RedirectToPage("/Index");
        }
    }
}

