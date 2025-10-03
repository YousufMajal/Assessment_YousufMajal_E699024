namespace Infrastructure.Outbox;

// Configuration options for the Outbox service.

public sealed class OutboxProcessorOptions
{
    public const string SectionName = "Config:OutboxProcessor";

    public int ProcessingIntervalSeconds { get; set; } = 30;

    public int BatchSize { get; set; } = 10;

    public int DefaultBatchSize { get; set; } = 50;

    public TimeSpan ProcessingInterval => TimeSpan.FromSeconds(ProcessingIntervalSeconds);
}