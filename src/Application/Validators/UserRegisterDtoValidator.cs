using Application.Dto;
using Application.ErrorMessages;
using FluentValidation;

namespace Application.Validators;

public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(EmailErrorMessages.Empty)
            .EmailAddress().WithMessage(EmailErrorMessages.NotValid);
        
        RuleFor(x => x.Password)
            .NotNull()
            .MinimumLength(8).WithMessage(PasswordErrorMessages.MinimumLength)
            .MaximumLength(64).WithMessage(PasswordErrorMessages.MaximumLength)
            .Matches("[A-Z]").WithMessage(PasswordErrorMessages.UppercaseLetter)
            .Matches("[a-z]").WithMessage(PasswordErrorMessages.LowercaseLetter)
            .Matches("[0-9]").WithMessage(PasswordErrorMessages.Digit)
            .Matches("[^a-zA-Z0-9]").WithMessage(PasswordErrorMessages.SpecialChars);

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(64);
    }
}