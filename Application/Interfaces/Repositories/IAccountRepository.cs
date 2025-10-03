using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAccountRepository
{
    Task<BankAccount?> GetByIdAsync(Guid id);

    void Add(BankAccount account);

    void Remove(BankAccount account);

    void Update(BankAccount account);
}