namespace Infrastructure.Outbox;

// Configuration options for the Outbox service.

public sealed class OutboxOptions
{
    public const string SectionName = "Config:OutboxProcessor";

    public int ProcessingIntervalSeconds { get; set; } = 30;

    public int BatchSize { get; set; } = 10;

    public TimeSpan ProcessingInterval => TimeSpan.FromSeconds(ProcessingIntervalSeconds);
}