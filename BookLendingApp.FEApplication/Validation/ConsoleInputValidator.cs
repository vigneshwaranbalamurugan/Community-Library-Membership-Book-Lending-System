using System.Globalization;
using System.Text.RegularExpressions;
using BookLendingApp.ModelLibrary.Exceptions;

namespace BookLendingApp.FEApplication.Validation
{
    public static class ConsoleInputValidator
    {
        private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

        public static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                try
                {
                    return ValidateRequiredString(Console.ReadLine(), prompt);
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static string? ReadOptionalString(string prompt)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        public static string ReadRequiredStringWithDefault(string prompt, string currentValue)
        {
            while (true)
            {
                Console.WriteLine($"{prompt} (current: {currentValue}) - press Enter to keep:");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                try
                {
                    return ValidateRequiredString(input, prompt);
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static string? ReadOptionalStringWithDefault(string prompt, string? currentValue)
        {
            Console.WriteLine($"{prompt} (current: {currentValue ?? "<empty>"}) - press Enter to keep, type '-' to clear:");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return currentValue;
            }

            if (input.Trim() == "-")
            {
                return null;
            }

            return input.Trim();
        }

        public static string ReadEmail(string prompt)
        {
            while (true)
            {
                var email = ReadRequiredString(prompt);
                if (EmailRegex.IsMatch(email))
                {
                    return email;
                }

                Console.WriteLine("Enter a valid email format, for example: user@example.com");
            }
        }

        public static string ReadEmailWithDefault(string prompt, string currentValue)
        {
            while (true)
            {
                var email = ReadRequiredStringWithDefault(prompt, currentValue);
                if (EmailRegex.IsMatch(email))
                {
                    return email;
                }

                Console.WriteLine("Enter a valid email format, for example: user@example.com");
            }
        }

        public static string ReadMobileNumber(string prompt)
        {
            while (true)
            {
                var mobile = ReadRequiredString(prompt);
                if (TryNormalizeMobileNumber(mobile, out var normalized))
                {
                    return normalized;
                }

                Console.WriteLine("Enter a valid mobile number (10 to 15 digits, optional leading +). Example: +919876543210");
            }
        }

        public static string? ReadOptionalMobileNumber(string prompt)
        {
            while (true)
            {
                var mobile = ReadOptionalString(prompt);
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    return null;
                }

                if (TryNormalizeMobileNumber(mobile, out var normalized))
                {
                    return normalized;
                }

                Console.WriteLine("Enter a valid mobile number (10 to 15 digits, optional leading +). Example: +919876543210");
            }
        }

        public static string? ReadOptionalMobileNumberWithDefault(string prompt, string? currentValue)
        {
            while (true)
            {
                var mobile = ReadOptionalStringWithDefault(prompt, currentValue);
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    return null;
                }

                if (TryNormalizeMobileNumber(mobile, out var normalized))
                {
                    return normalized;
                }

                Console.WriteLine("Enter a valid mobile number (10 to 15 digits, optional leading +). Example: +919876543210");
            }
        }

        public static Guid ReadGuid(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                try
                {
                    return ValidateGuid(Console.ReadLine(), prompt);
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static int ReadInt(string prompt, int? minValue = null, int? maxValue = null)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                try
                {
                    var value = ValidateInt(Console.ReadLine(), prompt, minValue, maxValue);
                    return value;
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static int ReadIntWithDefault(string prompt, int currentValue, int? minValue = null, int? maxValue = null)
        {
            while (true)
            {
                Console.WriteLine($"{prompt} (current: {currentValue}) - press Enter to keep:");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                try
                {
                    return ValidateInt(input, prompt, minValue, maxValue);
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static decimal ReadDecimal(string prompt, decimal? minValue = null, decimal? maxValue = null)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                try
                {
                    return ValidateDecimal(Console.ReadLine(), prompt, minValue, maxValue);
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static decimal ReadDecimalWithDefault(string prompt, decimal currentValue, decimal? minValue = null, decimal? maxValue = null)
        {
            while (true)
            {
                Console.WriteLine($"{prompt} (current: {currentValue}) - press Enter to keep:");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                try
                {
                    return ValidateDecimal(input, prompt, minValue, maxValue);
                }
                catch (InputValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static DateTime ReadDateTime(string prompt, DateTime? minValue = null, DateTime? maxValue = null)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = Console.ReadLine();

                if (!DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var value))
                {
                    Console.WriteLine("Enter a valid date in yyyy-MM-dd or a standard date/time format.");
                    continue;
                }

                if (minValue.HasValue && value < minValue.Value)
                {
                    Console.WriteLine($"{prompt} must be on or after {minValue.Value:d}.");
                    continue;
                }

                if (maxValue.HasValue && value > maxValue.Value)
                {
                    Console.WriteLine($"{prompt} must be on or before {maxValue.Value:d}.");
                    continue;
                }

                return value;
            }
        }

        public static bool ReadYesNo(string prompt, bool defaultValue = false)
        {
            while (true)
            {
                var suffix = defaultValue ? "[Y/n]" : "[y/N]";
                Console.WriteLine($"{prompt} {suffix}");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue;
                }

                if (input.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) || input.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (input.Trim().Equals("n", StringComparison.OrdinalIgnoreCase) || input.Trim().Equals("no", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                Console.WriteLine("Please enter yes or no.");
            }
        }

        public static TEnum ReadEnum<TEnum>(string prompt) where TEnum : struct, Enum
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = Console.ReadLine();
                if (Enum.TryParse<TEnum>(input, true, out var parsed))
                {
                    return parsed;
                }

                Console.WriteLine($"Invalid {typeof(TEnum).Name} value.");
            }
        }

        public static T PromptSelection<T>(string title, IReadOnlyList<T> items, Func<T, string> formatter)
        {
            if (items.Count == 0)
            {
                throw new InvalidSelectionException(title);
            }

            Console.WriteLine(title);
            for (var index = 0; index < items.Count; index++)
            {
                Console.WriteLine($"{index + 1}. {formatter(items[index])}");
            }

            while (true)
            {
                Console.WriteLine("Enter choice number:");
                var input = Console.ReadLine();
                if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var selection) && selection >= 1 && selection <= items.Count)
                {
                    return items[selection - 1];
                }

                Console.WriteLine("Invalid selection.");
            }
        }

        public static TEnum PromptEnumSelection<TEnum>(string title) where TEnum : struct, Enum
        {
            var values = Enum.GetValues<TEnum>();
            Console.WriteLine(title);

            for (var index = 0; index < values.Length; index++)
            {
                Console.WriteLine($"{index + 1}. {values[index]}");
            }

            while (true)
            {
                Console.WriteLine("Enter choice number:");
                var input = Console.ReadLine();
                if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var selection) && selection >= 1 && selection <= values.Length)
                {
                    return values[selection - 1];
                }

                Console.WriteLine("Invalid selection.");
            }
        }

        public static TEnum PromptEnumSelectionWithDefault<TEnum>(string title, TEnum currentValue) where TEnum : struct, Enum
        {
            var values = Enum.GetValues<TEnum>();
            Console.WriteLine($"{title} (current: {currentValue}) - press Enter to keep:");

            for (var index = 0; index < values.Length; index++)
            {
                Console.WriteLine($"{index + 1}. {values[index]}");
            }

            while (true)
            {
                Console.WriteLine("Enter choice number:");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var selection) && selection >= 1 && selection <= values.Length)
                {
                    return values[selection - 1];
                }

                Console.WriteLine("Invalid selection.");
            }
        }

        private static string ValidateRequiredString(string? input, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new RequiredInputException(fieldName);
            }

            return input.Trim();
        }

        private static Guid ValidateGuid(string? input, string fieldName)
        {
            if (!Guid.TryParse(input, out var value))
            {
                throw new InvalidGuidInputException(fieldName);
            }

            return value;
        }

        private static int ValidateInt(string? input, string fieldName, int? minValue, int? maxValue)
        {
            if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                throw new InvalidNumberInputException(fieldName);
            }

            if (minValue.HasValue && value < minValue.Value)
            {
                throw new InputValidationException($"{fieldName} must be at least {minValue.Value}.");
            }

            if (maxValue.HasValue && value > maxValue.Value)
            {
                throw new InputValidationException($"{fieldName} must be at most {maxValue.Value}.");
            }

            return value;
        }

        private static decimal ValidateDecimal(string? input, string fieldName, decimal? minValue, decimal? maxValue)
        {
            if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                throw new InvalidNumberInputException(fieldName);
            }

            if (minValue.HasValue && value < minValue.Value)
            {
                throw new InputValidationException($"{fieldName} must be at least {minValue.Value}.");
            }

            if (maxValue.HasValue && value > maxValue.Value)
            {
                throw new InputValidationException($"{fieldName} must be at most {maxValue.Value}.");
            }

            return value;
        }

        private static bool TryNormalizeMobileNumber(string input, out string normalized)
        {
            normalized = string.Empty;
            var trimmed = input.Trim();
            var hasPlus = trimmed.StartsWith('+');

            var chars = trimmed.Where(char.IsDigit).ToArray();
            if (chars.Length < 10 || chars.Length > 15)
            {
                return false;
            }

            normalized = (hasPlus ? "+" : string.Empty) + new string(chars);
            return true;
        }
    }
}