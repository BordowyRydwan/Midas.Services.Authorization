using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<bool> RegisterNewUser(UserRegisterDto user)
    {
        var userEntity = _mapper.Map<UserRegisterDto, User>(user);

        try
        {
            await _userRepository.AddNewUser(userEntity).ConfigureAwait(false);
        }
        catch (UserException)
        {
            return false;
        }

        return true;
    }
}