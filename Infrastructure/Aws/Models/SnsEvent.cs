namespace Infrastructure.Aws.Models;

// contract for AWS SNS event publishing.
// Represents the structure required for publishing events to SNS topics.
public sealed record SnsEvent(
    string EventType,
    string TopicArn,
    object Payload,
    string? Subject = null,
    Dictionary<string, string>? MessageAttributes = null,
    DateTime? ScheduledTime = null
);