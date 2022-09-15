using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;

public class FamilyService : IFamilyService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IMapper _mapper;

    public FamilyService(IFamilyRepository familyRepository, IMapper mapper)
    {
        _familyRepository = familyRepository;
        _mapper = mapper;
    }
    
    public async Task<AddNewFamilyReturnDto> AddNewFamily(AddNewFamilyDto dto)
    {
        var familyEntity = _mapper.Map<AddNewFamilyDto, Family>(dto);
        var familyId = await _familyRepository.AddNewFamily(familyEntity, dto.FounderId);

        return new AddNewFamilyReturnDto
        {
            Id = familyId,
            Name = dto.Name
        };
    }

    public async Task<bool> DeleteFamily(ulong id)
    {
        return await _familyRepository.DeleteFamily(id).ConfigureAwait(false);
    }
}