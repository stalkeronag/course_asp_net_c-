using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Intro.Web
{
    [ApiController]
    [Route("calculator")]
    public class CalculatorController : ControllerBase
    {
        [HttpGet("add")]
        public float AddNumbers(float x, float y)
        {
            Calculator calculator = new Calculator();

            float result = calculator.Add(x, y);

            return result;
        }

        [HttpGet("sub")]
        public float SubNumbers(float x, float y)
        {
            Calculator calculator = new Calculator();

            float result = calculator.Sub(x, y);

            return result;
        }

        [HttpGet("mul")]
        public float MulNumbers(float x, float y)
        {
            Calculator calculator = new Calculator();

            float result = calculator.Mul(x, y);

            return result;
        }

        [HttpGet("div")]
        public float DivNumbers(float x, float y) 
        {
            Calculator calculator = new Calculator();

            if (y == 0)
                return -1;
            else
                return calculator.Div(x, y);

        }
    }
}
