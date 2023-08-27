using Dotnet.Web.Models;
using FluentValidation;

namespace Dotnet.Web.Validation;

public class ProductValidator : AbstractValidator<Product> 
{
    public ProductValidator()
    {
        RuleFor(product => product.DiscountPercent).GreaterThan(-1).LessThan(101).WithMessage("value discount price must have between 0 to 100");
        RuleFor(product => product.Price).GreaterThan(-1).WithMessage("value price must have more that zero");
    }
}