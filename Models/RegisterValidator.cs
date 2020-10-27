using FluentValidation;
using System.IO;

namespace PasteBin.Models
{
    public class RegisterValidator: AbstractValidator<ApplicationUser>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid e-mail Address.");
            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password must be atleast 8 characters long.");
            RuleFor(x => x.Email).Custom((val, context) =>
            {
                if (File.Exists(Path.Combine(Locations.UsersLocation, val)))
                {
                    context.AddFailure("This e-mail is already in use.");
                }
            });
        }
    }
}