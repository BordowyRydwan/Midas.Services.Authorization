using Application.Interfaces;

namespace Application.Dto;

public class UserFamilyRoleListDto : IListDto<UserFamilyRoleDto>
{
    public int Count { get; set; }
    public ICollection<UserFamilyRoleDto> Items { get; set; }
}