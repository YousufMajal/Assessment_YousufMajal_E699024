namespace Application.Events;

public sealed record WithdrawalPerformedIntegrationEvent(
    Guid EventId,
    Guid AccountId,
    decimal Amount,
    decimal PreviousBalance,
    decimal NewBalance,
    DateTime OccurredUtc);