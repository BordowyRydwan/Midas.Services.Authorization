namespace Application.Dto;

public class SetUserFamilyRoleDto
{
    public string Email { get; set; }
    public ulong FamilyId { get; set; }
    public ulong FamilyRoleId { get; set; }
}