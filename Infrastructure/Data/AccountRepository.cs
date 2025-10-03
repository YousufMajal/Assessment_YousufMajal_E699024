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

    public async Task<UserAccount?> GetByIdAsync(Guid id)
    {
        return await _context.Set<UserAccount>()
            .FirstOrDefaultAsync(a => a.accountId == id);
    }

    public void Add(UserAccount account)
    {
        _context.Set<UserAccount>().Add(account);
    }

    public void Remove(UserAccount account)
    {
        _context.Set<UserAccount>().Remove(account);
    }

    public void Update(UserAccount account)
    {
        _context.Set<UserAccount>().Update(account);
    }
}