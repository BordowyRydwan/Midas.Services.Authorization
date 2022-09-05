namespace Domain.Entities;

public class UserFamilyRole
{
    public ulong UserId { get; set; }
    public User User { get; set; }
    public ulong FamilyId { get; set; }
    public Family Family { get; set; }
    public ulong FamilyRoleId { get; set; }
    public FamilyRole FamilyRole { get; set; }
}