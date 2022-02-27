using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Abstractions.Services;
using Test.Banking.Api.Exceptions;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public async Task<ServiceResult<User?>> Create(User user)
    {
        var errors = new List<string>();
        User? newUser = null;
        
        try
        {
            newUser = await this.userRepository.Create(user);
        }
        catch (RepositoryException e)
        {
            errors.Add($"User Error: {e.Message}");
        }

        return new ServiceResult<User?>
        {
            Result = newUser,
            Errors = errors,
        };
    }

    public async Task<ServiceResult<User?>> Read(int id)
    {
        return new ServiceResult<User?>
        {
            Result = await this.userRepository.Read(id),
        };
    }
}