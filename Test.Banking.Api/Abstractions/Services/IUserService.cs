using Test.Banking.Api.Types;

namespace Test.Banking.Api.Abstractions.Services;

public interface IUserService
{
    Task<ServiceResult<User?>> Create(User user);

    Task<ServiceResult<User?>> Read(int id);
}