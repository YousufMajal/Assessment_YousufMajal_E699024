namespace Application.DTOs;

public sealed record WithdrawalEventData(
    Guid AccountId,
    decimal Amount,
    decimal PreviousBalance,
    decimal NewBalance,
    DateTime Timestamp,
    string EventVersion = "1.0");