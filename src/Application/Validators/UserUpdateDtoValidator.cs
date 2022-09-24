using Application.Dto;
using FluentValidation;

namespace Application.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.FirstName).MinimumLength(3).MaximumLength(64);
        RuleFor(x => x.LastName).MinimumLength(3).MaximumLength(64);
        RuleFor(x => x.BirthDate).LessThan(DateTime.Today);
    }
}