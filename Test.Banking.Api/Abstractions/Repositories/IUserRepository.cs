using Test.Banking.Api.Types;

namespace Test.Banking.Api.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> Create(User user);

    Task<User?> Read(int id);
}