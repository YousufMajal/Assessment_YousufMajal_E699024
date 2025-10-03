using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAccountRepository
{
    Task<UserAccount?> GetByIdAsync(Guid id);

    void Add(UserAccount account);

    void Remove(UserAccount account);

    void Update(UserAccount account);
}