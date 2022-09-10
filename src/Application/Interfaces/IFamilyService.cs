using Application.Dto;

namespace Application.Interfaces;

public interface IFamilyService : IInternalService
{
    public Task AddNewFamily(AddNewFamilyDto dto);
}