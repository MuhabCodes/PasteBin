using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PasteBin.Models;
using FluentValidation;


namespace PasteBin.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        private readonly IValidator<ApplicationUser> _validator;

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; } = new();

        public RegisterModel(ILogger<RegisterModel> logger, IValidator<ApplicationUser> validator)
        {
            _logger = logger;
            _validator = validator;

            if (!Directory.Exists(Locations.UsersLocation))
            {
                Directory.CreateDirectory(Locations.UsersLocation);
            }
        }
        
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            RegisterValidator validator = new();
            var result = validator.Validate(ApplicationUser);

            if (!result.IsValid)
            {
                foreach (var message in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.ErrorMessage);
                }
                return Page();
            }

            ApplicationUser.Password = Hashing.hash(ApplicationUser.Password, ApplicationUser.Email);

            string jsonString = JsonSerializer.Serialize(ApplicationUser);
            
            string fileName = $"{ApplicationUser.Email}.json";            
            string userPath = Path.Combine(Locations.UsersLocation, fileName);

            System.IO.File.WriteAllText(userPath,jsonString);

            _logger.LogInformation("A new account has been created user {Email}",ApplicationUser.Email); 
            return RedirectToPage("/Index");
        }
    }
}

