using Application.Dto;
using FluentValidation;

namespace Application.Validators;

public class UserUpdateEmailDtoValidator : AbstractValidator<UserUpdateEmailDto>
{
    public UserUpdateEmailDtoValidator()
    {
        RuleFor(x => x.OldEmail).EmailAddress().NotEqual(x => x.NewEmail);
        RuleFor(x => x.NewEmail).EmailAddress();
    }
}