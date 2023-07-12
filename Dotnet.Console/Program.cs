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

            if (String.IsNullOrEmpty(expression))
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

            int index = 0;
            string[] numbers = new string[2];
            char operation = ' ';
            bool timeForOperaiton = false;

            for (int i = 0; i < expression.Length; i++)
            {
                if (index == 2)
                    break;

                if (expression[i] == ' ')
                {
                    continue;
                }

                if (digits.Contains(expression[i]))
                {
                    int pos = i;
                    numbers[index] = SelectFirstNumber(expression, pos);
                    i += numbers[index].Length - 1;
                    index++;
                    timeForOperaiton = true;
                    continue;
                }

                bool isSign = expression[i] == '+' || expression[i] == '-';
                if (isSign && digits.Contains(expression[i + 1]) && !timeForOperaiton)
                {
                    i++;
                    int pos = i;
                    numbers[index] = expression[i - 1] + SelectFirstNumber(expression, pos);
                    i += numbers[index].Length - 2;
                    index++;
                    timeForOperaiton = true;
                    continue;
                }

                if (operations.Contains(expression[i]) && timeForOperaiton)
                {
                    operation = expression[i];
                    timeForOperaiton = false;
                    continue;
                }
            }

            float operandOne = float.Parse(numbers[0]);
            float operandTwo = float.Parse(numbers[1]);

            try
            {
                System.Console.WriteLine(calc.Calculate(operandOne, operation, operandTwo));
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static String SelectFirstNumber(String source, int pos)
        {
            String result = "";
            bool isFloat = true;
            for (int i = pos; i < source.Length; i++)
            {
                int code = (int)source[i];
                if ((code >= 48 && code <= 57) || (code == 44 && isFloat))
                {
                    if (code == 46)
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
