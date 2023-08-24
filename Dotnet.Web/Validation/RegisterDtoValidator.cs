using Dotnet.Web.Dto;
using FluentValidation;

namespace Dotnet.Web.Validation;

public class RegisterDtoValidator : AbstractValidator<RegisterDto> 
{
    public RegisterDtoValidator()
    {
        RuleFor(registerDto => registerDto.Password).Must(password => IsPasswordValid(password));
        RuleFor(registerDto => registerDto.Email).Must(email => IsEmailValid(email));
    }

    private bool IsEmailValid(string email)
    {
        return email.EndsWith(".com");
    }

    private bool IsPasswordValid(string password)  
    {
        char[] upperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] digits = "1234567890".ToCharArray();
        char[] lowerLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
        bool existUpperLetters = password.Intersect(upperLetters).Count() > 0;
        bool existLowerLetters = password.Intersect(lowerLetters).Count() > 0;
        bool existDigits = password.Intersect(digits).Count() > 0;
        return existUpperLetters && existLowerLetters && existDigits;
    }
}