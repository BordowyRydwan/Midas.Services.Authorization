using Application.Dto;

namespace Application.Interfaces;

public interface IFamilyService : IInternalService
{
    public Task<AddNewFamilyReturnDto> AddNewFamily(AddNewFamilyDto dto);
    public Task<bool> DeleteFamily(ulong id);
}