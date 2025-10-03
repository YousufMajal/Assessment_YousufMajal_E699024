using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using System.Text.Json;

namespace Infrastructure.Outbox;

public sealed class OutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly ApplicationDbContext _db;

    public OutboxWriter(ApplicationDbContext db) => _db = db;

    public void Enqueue<T>(T message, string eventType)
    {
        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = eventType,
            Content = JsonSerializer.Serialize(message, SerializerOptions),
            OccurredOnUtc = DateTime.UtcNow
        };
        _db.Set<OutboxMessage>().Add(outbox);
    }
}