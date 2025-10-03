namespace Application.Events;

public sealed record WithdrawalEvent(
    Guid EventId,
    Guid AccountId,
    decimal Amount,
    decimal PreviousBalance,
    decimal NewBalance,
    DateTime OccurredUtc);