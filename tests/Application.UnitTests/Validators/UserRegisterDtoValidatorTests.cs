using Application.Dto;
using Application.ErrorMessages;
using Application.Validators;

namespace Application.UnitTests.Validators;

[TestFixture]
public class UserRegisterDtoValidatorTests
{
    private readonly UserRegisterDtoValidator _validator;
    
    public UserRegisterDtoValidatorTests()
    {
        _validator = new UserRegisterDtoValidator();
    }

    [Test]
    public void ShouldThrowErrorForEmptyEmail()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == EmailErrorMessages.Empty));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForInvalidEmail()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@@testpl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == EmailErrorMessages.NotValid));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForEmptyFirstName()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.PropertyName == "FirstName"));
        });
    }
    [Test]
    public void ShouldThrowErrorForEmptyLastName()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.PropertyName == "LastName"));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForNullPassword()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12)
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.PropertyName == "Password"));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForTooShortPassword()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "test"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == PasswordErrorMessages.MinimumLength));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForPasswordContainingNoUppercaseLetter()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@wsx"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == PasswordErrorMessages.UppercaseLetter));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForPasswordContainingNoLowercaseLetter()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "ZAQ1@WSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == PasswordErrorMessages.LowercaseLetter));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForPasswordContainingNoDigit()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaqqqq@WSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == PasswordErrorMessages.Digit));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForPasswordContainingNoSpecialChar()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1aWSX"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == PasswordErrorMessages.SpecialChars));
        });
    }
    
    [Test]
    public void ShouldBeValidForCorrectModel()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSX"
        };
        var validation = _validator.Validate(testInstance);
        Assert.That(validation.IsValid, Is.True);
    }
    
    [Test]
    public void ShouldThrowErrorForTooLongPassword()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "test",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSXkjlhqdjklwqhdjwqhdljkwqhdlkjqhwkjdhwqdhjklwqhdlkjqhldkhqwdlkjwdjkwqhdkljqwhjdklhwljkdwqdhlwq"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Errors.Any(x => x.ErrorMessage == PasswordErrorMessages.MaximumLength));
        });
    }
    
    [Test]
    public void ShouldThrowErrorForTooLongFirstName()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            FirstName = "testkljlkjlkjkljlkjkljlkl;jwfkjwefljflkfjek;fjkewjf;klewjf;ewj;lfwjefl;ljf;jew;kfljk;le",
            LastName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSXk"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.That(validation.IsValid, Is.False);
    }
    
    [Test]
    public void ShouldThrowErrorForTooLongLastName()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "wrongemail@test.pl",
            LastName = "testkljlkjlkjkljlkjkljlkl;jwfkjwefljflkfjek;fjkewjf;klewjf;ewj;lfwjefl;ljf;jew;kfljk;le",
            FirstName = "test",
            BirthDate = new DateTime(1998, 10, 12),
            Password = "zaq1@WSXk"
        };
        var validation = _validator.Validate(testInstance);
        
        Assert.That(validation.IsValid, Is.False);
    }
}