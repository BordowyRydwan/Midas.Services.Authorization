using Application.Dto;

namespace Application.Interfaces;

public interface IFamilyService : IInternalService
{
    public Task<AddNewFamilyReturnDto> AddNewFamily(AddNewFamilyDto dto);
    public Task<bool> DeleteFamily(ulong id);
    public Task<bool> AddUserToFamily(AddUserToFamilyDto dto);
    public Task<bool> DeleteUserFromFamily(DeleteUserFromFamilyDto dto);
    public Task<bool> SetUserFamilyRole(SetUserFamilyRoleDto dto);
}