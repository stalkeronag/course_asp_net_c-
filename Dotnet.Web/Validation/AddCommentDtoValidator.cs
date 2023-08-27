using Dotnet.Web.Dto;
using FluentValidation;

namespace Dotnet.Web.Validation;

public class AddCommentDtoValidator : AbstractValidator<AddCommentDto> 
{
    public AddCommentDtoValidator()
    {
        RuleFor(addComment => addComment.Rating).GreaterThan(-1).LessThan(6).WithMessage("Rating value must from 0 to 5");
        RuleFor(addComment => addComment.Text).MaximumLength(200).WithMessage("comment length can't more 200 characters");
    }
}