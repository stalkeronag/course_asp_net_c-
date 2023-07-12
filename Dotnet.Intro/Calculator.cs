namespace Dotnet.Intro;

public class Calculator
{
    public float Calculate(float operandOne, char operation, float operandTwo)
    {
        try
        {
            switch (operation)
            {
                case '+':
                    return Add(operandOne, operandTwo);
                case '-':
                    return Sub(operandOne, operandTwo);
                case '*':
                    return Mul(operandOne, operandTwo);
                case '/':
                    return Div(operandOne, operandTwo);
                default:
                    break;
            }
            return -1;
        }
        catch (Exception)
        {
            throw new DivideByZeroException();
        }
    }

    public float Add(float x, float y)
    {
        return x + y;
    }

    public float Sub(float x, float y)
    {
        return x - y;
    }

    public float Mul(float x, float y)
    {
        return x * y;
    }

    public float Div(float x, float y)
    {
        return x / y;
    }
}