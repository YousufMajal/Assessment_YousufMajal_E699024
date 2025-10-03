namespace Application.DTOs;

/// Result DTO returned to API consumers after a withdrawal.
public sealed record WithdrawalResultDto(
    Guid AccountId,
    decimal PreviousBalance,
    decimal WithdrawnAmount,
    decimal NewBalance,
    string EventStatus,
    DateTime TimestampUtc);