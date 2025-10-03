using Application.Interfaces;

namespace Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Implements IUnitOfWork Contract
    // used by Outbox Service to persist changes
    // also used by UnitOfWorkBehaviourPipeline => automatically exec's SaveChangesAsync for a CQRS Command (Ignores queries)
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}