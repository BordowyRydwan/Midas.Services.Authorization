using Application.Dto;
using Application.ErrorMessages;
using FluentValidation;

namespace Application.Validators;

public class UserUpdatePasswordDtoValidator : AbstractValidator<UserUpdatePasswordDto>
{
    public UserUpdatePasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(EmailErrorMessages.NotValid);
        
        RuleFor(x => x.NewPassword)
            .NotNull()
            .MinimumLength(8).WithMessage(PasswordErrorMessages.MinimumLength)
            .MaximumLength(64).WithMessage(PasswordErrorMessages.MaximumLength)
            .Matches("[A-Z]").WithMessage(PasswordErrorMessages.UppercaseLetter)
            .Matches("[a-z]").WithMessage(PasswordErrorMessages.LowercaseLetter)
            .Matches("[0-9]").WithMessage(PasswordErrorMessages.Digit)
            .Matches("[^a-zA-Z0-9]").WithMessage(PasswordErrorMessages.SpecialChars);
    }
}