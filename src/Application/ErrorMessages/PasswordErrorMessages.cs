namespace Application.ErrorMessages;

public static class PasswordErrorMessages
{
    public const string MinimumLength = "Password should be at least 8 letters length";
    public const string MaximumLength = "Password should be at most 64 letters length";
    public const string UppercaseLetter = "Password should contain uppercase letter";
    public const string LowercaseLetter = "Password should contain lowercase letter";
    public const string Digit = "Password should contain at least one digit";
    public const string SpecialChars = "Password should contain a special char";
}