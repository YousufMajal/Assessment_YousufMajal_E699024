using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccount?> GetByIdAsync(Guid id)
    {
        return await _context.Set<BankAccount>()
            .FirstOrDefaultAsync(a => a.AccountId == id);
    }

    public void Add(BankAccount account)
    {
        _context.Set<BankAccount>().Add(account);
    }

    public void Remove(BankAccount account)
    {
        _context.Set<BankAccount>().Remove(account);
    }

    public void Update(BankAccount account)
    {
        _context.Set<BankAccount>().Update(account);
    }
}