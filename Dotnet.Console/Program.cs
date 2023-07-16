using System;
using Dotnet.Intro;

namespace Dotnet.Console
{
    public class Program
    {

        public static int Main(string[] args)
        {
            Calculator calc = new Calculator();
            string expression = System.Console.ReadLine();

            if (string.IsNullOrEmpty(expression))
            {
                return -1;
            }

            char[] operations = new char[4]
            {
                '+',
                '-',
                '/',
                '*'
            };

            char[] digits = new char[10]
            {
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9'
            };

            int countReadNumbers = 0;
            string[] numbers = new string[2];
            char operation = ' ';
            bool isOperation = false;

            try
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    if (countReadNumbers == 2)
                        break;

                    if (expression[i] == ' ')
                    {
                        continue;
                    }

                    if (digits.Contains(expression[i]))
                    {
                        int pos = i;
                        numbers[countReadNumbers] = SelectNumber(expression, pos);
                        i += numbers[countReadNumbers].Length - 1;
                        countReadNumbers++;
                        isOperation = true;
                        continue;
                    }

                    bool isSign = expression[i] == '+' || expression[i] == '-';
                    if (isSign && digits.Contains(expression[i + 1]) && !isOperation)
                    {
                        i++;
                        int pos = i;
                        numbers[countReadNumbers] = expression[i - 1] + SelectNumber(expression, pos);
                        i += numbers[countReadNumbers].Length - 2;
                        countReadNumbers++;
                        isOperation = true;
                        continue;
                    }

                    if (operations.Contains(expression[i]) && isOperation)
                    {
                        operation = expression[i];
                        isOperation = false;
                        continue;
                    }
                }

                float operandOne = float.Parse(numbers[0]);
                float operandTwo = float.Parse(numbers[1]);

                System.Console.WriteLine(calc.Calculate(operandOne, operation, operandTwo));
                return 0;
            }
            catch (DivideByZeroException)
            {
                return -1;
            }

        }

        public static string SelectNumber(string source, int pos)
        {
            int codeAsciiZero = 48;
            int codeAsciiNine = 57;
            int codeAsciiComma = 44;
            string result = string.Empty;
            bool isFloat = true;
            for (int i = pos; i < source.Length; i++)
            {
                int code = source[i];
                if ((code >= codeAsciiZero && code <= codeAsciiNine) || (code == codeAsciiComma && isFloat))
                {
                    if (code == codeAsciiComma)
                    {
                        isFloat = false;
                    }
                    result += source[i];
                }
                else
                {
                    break;
                }
            }
            return result;
        }
    }



}
