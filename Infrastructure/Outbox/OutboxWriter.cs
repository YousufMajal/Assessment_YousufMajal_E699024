using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using System.Text.Json;

namespace Infrastructure.Outbox;

public sealed class OutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IOutboxRepository _outboxRepository;

    public OutboxWriter(IOutboxRepository outboxRepository) => _outboxRepository = outboxRepository;

    public void Enqueue<T>(T message, string eventType)
    {
        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = eventType,
            Content = JsonSerializer.Serialize(message, SerializerOptions),
            OccurredOnUtc = DateTime.UtcNow
        };
        _outboxRepository.Add(outbox);
    }
}