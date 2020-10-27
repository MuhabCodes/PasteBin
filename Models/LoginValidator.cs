
using System.IO;
using System.Text.Json;
using FluentValidation;

namespace PasteBin.Models
{
    public class LoginValidator: AbstractValidator<ApplicationUser>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid e-mail Address.");
            RuleFor(x => x).Custom((val, context) =>
            {
                string jsonPath = Path.Combine(Locations.UsersLocation, $"{val.Email}.json");
                if (!File.Exists(jsonPath))
                {
                    context.AddFailure("This e-mail is not registered.");
                    return;
                }
                string jsonString = File.ReadAllText(jsonPath);
                ApplicationUser user = JsonSerializer.Deserialize<ApplicationUser>(jsonString);

                if (user.Password != Hashing.hash(val.Password,val.Email))
                {
                    context.AddFailure("The e-mail or password that you have entered is incorrect.");
                }

            });
        }
    }
}