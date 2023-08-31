using Dotnet.Web.Admin.Dto;
using FluentValidation;

namespace Dotnet.Web.Admin.Validation;

public class RegisterDtoValidator : AbstractValidator<RegisterDto> 
{
    private int minimalLengthPassword = 8;

    private int minimalLengthUserName = 4;
    public RegisterDtoValidator()
    {
        RuleFor(rd => rd.Email).NotEmpty().EmailAddress();
        RuleFor(rd => rd.Password).NotEmpty().MinimumLength(minimalLengthPassword);
        RuleFor(rd => rd.UserName).NotEmpty().MinimumLength(minimalLengthUserName);
    }
}