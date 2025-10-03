using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOutboxRepository
{
    Task<OutboxMessage?> GetByIdAsync(Guid id);

    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 50);

    Task<IEnumerable<OutboxMessage>> GetProcessedMessagesAsync(DateTime? fromDate = null, int batchSize = 50);

    Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int batchSize = 50);

    void Add(OutboxMessage message);

    void Update(OutboxMessage message);

    void Remove(OutboxMessage message);

    Task<int> CountUnprocessedAsync();

    Task<int> CountFailedAsync();

    Task MarkAsProcessedAsync(Guid id, DateTime processedOnUtc);

    Task MarkAsFailedAsync(Guid id, string error);
}