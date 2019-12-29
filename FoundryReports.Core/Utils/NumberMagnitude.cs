using System;

namespace FoundryReports.Core.Utils
{
    internal class NumberMagnitude
    {
        public int NextBiggestMagnitude { get; }

        private NumberMagnitude(int nextBiggestMagnitude)
        {
            NextBiggestMagnitude = nextBiggestMagnitude;
        }

        public static NumberMagnitude FromNumbers(double number)
        {
            var isNegative = number < 0;
            var asAbsoluteInt = AsAbsoluteInt(number);
            if (asAbsoluteInt <= 10)
            {
                if (asAbsoluteInt < 4)
                {
                    if (asAbsoluteInt == 0)
                        return new NumberMagnitude(0);
                    else
                        return new NumberMagnitude(5 * (isNegative ? -1 : 1));
                }
                else
                    return new NumberMagnitude(10 * (isNegative ? -1 : 1));
            }

            var amountOfDigits = AmountOfDigits(number);
            var leftMostDigit = LeftmostDigit(number);
            var secondLeftMostDigit = SecondLeftmostDigit(number);

            int leftMostTwoDigitsOfMagnitude;
            if (secondLeftMostDigit < 5)
                leftMostTwoDigitsOfMagnitude = CombineToNumber((uint) leftMostDigit, 5);
            else
                leftMostTwoDigitsOfMagnitude = CombineToNumber((uint) leftMostDigit + 1, 0);

            var magnitude = AddZeros(leftMostTwoDigitsOfMagnitude, amountOfDigits - 2);

            return new NumberMagnitude(magnitude * (isNegative ? -1 : 1));
        }

        private static int CombineToNumber(uint leftDigit, uint rightDigit)
        {
            var asString = leftDigit.ToString() + rightDigit;
            return int.Parse(asString);
        }

        private static int AddZeros(int toNumber, int amountOfZeros)
        {
            var asString = toNumber.ToString();
            asString = asString.PadRight(asString.Length + amountOfZeros, '0');

            return int.Parse(asString);
        }

        private static int LeftmostDigit(double ofNumber)
        {
            var asInt = AsAbsoluteInt(ofNumber);
            var split = Math.Abs(asInt);

            while (split >= 10)
                split /= 10;

            return split;
        }

        private static int SecondLeftmostDigit(double ofNumber)
        {
            var asInt = AsAbsoluteInt(ofNumber);
            var asString = asInt.ToString();
            asString = asString.Substring(1, 1);
            if (string.IsNullOrEmpty(asString))
                return asInt;

            return LeftmostDigit(int.Parse(asString));
        }

        private static int AsAbsoluteInt(double ofNumber) => (int) Math.Round(Math.Abs(ofNumber));

        private static int AmountOfDigits(double number) => AsAbsoluteInt(number).ToString().Length;
    }
}