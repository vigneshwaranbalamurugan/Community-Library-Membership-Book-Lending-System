namespace BookLendingApp.ModelLibrary.Exceptions
{
    public class InputValidationException : Exception
    {
        public InputValidationException(string message) : base(message)
        {
        }
    }

    public class RequiredInputException : InputValidationException
    {
        public RequiredInputException(string fieldName) : base($"{fieldName} is required.")
        {
        }
    }

    public class InvalidGuidInputException : InputValidationException
    {
        public InvalidGuidInputException(string fieldName) : base($"{fieldName} must be a valid GUID.")
        {
        }
    }

    public class InvalidNumberInputException : InputValidationException
    {
        public InvalidNumberInputException(string fieldName) : base($"{fieldName} must be a valid number.")
        {
        }
    }

    public class InvalidSelectionException : InputValidationException
    {
        public InvalidSelectionException(string fieldName) : base($"{fieldName} selection is invalid.")
        {
        }
    }
}