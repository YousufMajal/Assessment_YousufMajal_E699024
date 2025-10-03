namespace BankingAPI.Models;

// data required to initiate a withdrawal from an account.
public sealed record WithdrawalRequest(decimal Amount);