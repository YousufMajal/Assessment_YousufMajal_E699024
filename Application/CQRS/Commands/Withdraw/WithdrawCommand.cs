using Application.Abstractions;
using Application.DTOs;

namespace Application.CQRS.Commands.Withdraw;

/// Command to perform a withdrawal on an account.
public sealed record WithdrawCommand(Guid AccountId, decimal Amount) : ICommand<WithdrawalResultDto>;