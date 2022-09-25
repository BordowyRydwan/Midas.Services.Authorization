namespace Application.Dto;

public class UserDto
{
    public ulong Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime BirthDate { get; set; }
    public UserFamilyRoleListDto UserFamilyRoles { get; set; }
}