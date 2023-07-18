using Dotnet.Intro;

namespace Dotnet.Intro.Test
{
    public class CalculatorTest
    {
        [Fact]
        public void AddNumberTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Add(5, 12);

            Assert.Equal(17, result);
        }

        [Fact]
        public void SubNumberTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Sub(15, 20);

            Assert.Equal(-5, result);
        }

        [Fact]
        public void MulNumberTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Mul(4, 1.5f);

            Assert.Equal(6, result);
        }

        [Fact]
        public void DivNumberTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Div(15, 1.5f);

            Assert.Equal(10, result);
        }

        [Fact]
        public void DivideByZeroTest()
        {
            Calculator calculator = new Calculator();

            Action actionDivideZero = () => calculator.Div(5, 0);

            Assert.Throws<DivideByZeroException>(actionDivideZero);
        }

        [Fact]
        public void CalculateNoExistingOperationTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Calculate(5, '&', 7);

            Assert.Equal(-1, result);
        }

        [Fact]
        public void CalculateAddOperatorTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Calculate(2, '+', 2);

            Assert.Equal(4, result);
        }

        [Fact]
        public void CalculateSubtractOperatorTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Calculate(2, '-', 2);

            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateMultiplyOperatorTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Calculate(2, '*', 2);

            Assert.Equal(4, result);
        }

        [Fact]
        public void CalculateDivideOperatorTest()
        {
            Calculator calculator = new Calculator();

            float result = calculator.Calculate(2, '/', 2);

            Assert.Equal(1, result);
        }

        [Fact]
        public void CalculateDivideOperatorWithZeroDenominatorTest()
        {
            Calculator calculator = new Calculator();

            Action result = () => calculator.Calculate(2, '/', 0);

            Assert.Throws<DivideByZeroException>(result); 
        }
    }
}