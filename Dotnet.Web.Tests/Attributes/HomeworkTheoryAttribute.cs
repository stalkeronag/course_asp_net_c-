namespace Dotnet.Web.Tests.Attributes;

public class HomeworkTheoryAttribute: TheoryAttribute
{
    private readonly Homeworks _number;

    public HomeworkTheoryAttribute(Homeworks number)
    {
        _number = number;
    }

    public override string? Skip => HomeworkAttribute.GetSkip(_number);
}