using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly ApplicationDbContext _context;

    public OutboxRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OutboxMessage?> GetByIdAsync(Guid id)
    {
        return await _context.Set<OutboxMessage>()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 50)
    {
        return await _context.Set<OutboxMessage>()
            .Where(o => o.ProcessedOnUtc == null && o.Error == null)
            .OrderBy(o => o.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboxMessage>> GetProcessedMessagesAsync(DateTime? fromDate = null, int batchSize = 50)
    {
        var query = _context.Set<OutboxMessage>()
            .Where(o => o.ProcessedOnUtc != null);

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.ProcessedOnUtc >= fromDate);
        }

        return await query
            .OrderByDescending(o => o.ProcessedOnUtc)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int batchSize = 50)
    {
        return await _context.Set<OutboxMessage>()
            .Where(o => o.Error != null)
            .OrderBy(o => o.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync();
    }

    public void Add(OutboxMessage message)
    {
        _context.Set<OutboxMessage>().Add(message);
    }

    public void Update(OutboxMessage message)
    {
        _context.Set<OutboxMessage>().Update(message);
    }

    public void Remove(OutboxMessage message)
    {
        _context.Set<OutboxMessage>().Remove(message);
    }

    public async Task<int> CountUnprocessedAsync()
    {
        return await _context.Set<OutboxMessage>()
            .CountAsync(o => o.ProcessedOnUtc == null && o.Error == null);
    }

    public async Task<int> CountFailedAsync()
    {
        return await _context.Set<OutboxMessage>()
            .CountAsync(o => o.Error != null);
    }

    public async Task MarkAsProcessedAsync(Guid id, DateTime processedOnUtc)
    {
        var message = await GetByIdAsync(id);
        if (message != null)
        {
            message.ProcessedOnUtc = processedOnUtc;
            message.Error = null; // Clear any previous error
            Update(message);
        }
    }

    public async Task MarkAsFailedAsync(Guid id, string error)
    {
        var message = await GetByIdAsync(id);
        if (message != null)
        {
            message.Error = error;
            message.ProcessedOnUtc = null; // Clear processed timestamp if it was set
            Update(message);
        }
    }
}