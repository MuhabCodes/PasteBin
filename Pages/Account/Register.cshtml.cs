using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PasteBin.Models;


namespace PasteBin.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; } = new();

        public RegisterModel(ILogger<RegisterModel> logger)
        {
            _logger = logger;
        }
        
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            ApplicationUser.Password = Hashing.hash(ApplicationUser.Password, ApplicationUser.Email);

            string jsonString = JsonSerializer.Serialize(ApplicationUser);
            
            string fileName = $"{ApplicationUser.Email}.json";            
            string userPath = Path.Combine(Locations.UsersLocation,fileName);

            if (System.IO.File.Exists(userPath))
            {
                ModelState.AddModelError(string.Empty, "This Email is already registered");
                return Page(); 
            }  
            System.IO.File.WriteAllText(userPath,jsonString);

            _logger.LogInformation("A new account has been created user {Email}",ApplicationUser.Email); 
            return RedirectToPage("/Index");
        }
    }
}

