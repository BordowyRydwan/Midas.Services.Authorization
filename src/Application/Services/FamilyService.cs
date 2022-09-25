using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;

public class FamilyService : IFamilyService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public FamilyService(IFamilyRepository familyRepository, IUserRepository userRepository, IMapper mapper)
    {
        _familyRepository = familyRepository;
        _userRepository = userRepository;
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

    public async Task<bool> AddUserToFamily(AddUserToFamilyDto dto)
    {
        var userEntity = await _userRepository.GetUserByEmail(dto.Email).ConfigureAwait(false);
        var familyEntity = await _familyRepository.GetFamilyById(dto.FamilyId).ConfigureAwait(false);

        if (userEntity is null || familyEntity is null)
        {
            return false;
        }

        return await _familyRepository.AddUserToFamily(userEntity.Id, familyEntity.Id).ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserFromFamily(DeleteUserFromFamilyDto dto)
    {
        var userEntity = await _userRepository.GetUserByEmail(dto.Email).ConfigureAwait(false);
        var familyEntity = await _familyRepository.GetFamilyById(dto.FamilyId).ConfigureAwait(false);

        if (userEntity is null || familyEntity is null)
        {
            return false;
        }

        return await _familyRepository.DeleteUserFromFamily(userEntity.Id, familyEntity.Id).ConfigureAwait(false);
    }

    public async Task<bool> SetUserFamilyRole(SetUserFamilyRoleDto dto)
    {
        var userEntity = await _userRepository.GetUserByEmail(dto.Email).ConfigureAwait(false);
        
        if (userEntity is null)
        {
            return false;
        }
        
        var userFamilyRole = _mapper.Map<SetUserFamilyRoleDto, UserFamilyRole>(dto, opt =>
        {
            opt.AfterMap((_, dest) =>
            {
                dest.UserId = userEntity.Id;
                dest.User = userEntity;
            });
        });
        
        return await _familyRepository.SetUserFamilyRole(userFamilyRole).ConfigureAwait(false);
    }
}