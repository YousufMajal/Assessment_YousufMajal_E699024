using Application.Abstractions;
using Application.DTOs;
using Application.Errors;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Events;

namespace Application.CQRS.Commands.Withdraw;

internal sealed class WithdrawCommandHandler : ICommandHandler<WithdrawCommand, WithdrawalResultDto>
{
    private readonly IAccountRepository _accounts;
    private readonly IOutboxWriter _outbox;
    private static readonly Error AccountNotFound = AccountErrorResponses.AccountNotFoundError();
    private static readonly Error InsufficientFunds = AccountErrorResponses.InsufficientFundsError();

    public WithdrawCommandHandler(
        IAccountRepository accounts,
        IOutboxWriter outbox)
    {
        _accounts = accounts;
        _outbox = outbox;
    }

    public async Task<Result<WithdrawalResultDto>> Handle(WithdrawCommand command, CancellationToken cancellationToken)
    {
        // Check if account exists & get balance
        var account = await _accounts.GetByIdAsync(command.AccountId);
        if (account is null)
        {
            return Result.Failure<WithdrawalResultDto>(AccountNotFound);
        }
        var previous = account.Balance;

        if (previous < command.Amount)
        {
            return Result.Failure<WithdrawalResultDto>(InsufficientFunds);
        }

        account.Balance -= command.Amount;
        _accounts.Update(account);

        var withdrawalEvent = new WithdrawalEvent(
            EventId: Guid.NewGuid(),
            AccountId: command.AccountId,
            Amount: command.Amount,
            PreviousBalance: previous,
            NewBalance: account.Balance,
            OccurredUtc: DateTime.UtcNow);

        _outbox.Enqueue(withdrawalEvent, "banking.withdrawal.performed.v1");

        var dto = new WithdrawalResultDto(
            AccountId: command.AccountId,
            PreviousBalance: previous,
            WithdrawnAmount: command.Amount,
            NewBalance: account.Balance,
            EventStatus: "ENQUEUED",
            TimestampUtc: DateTime.UtcNow);

        return Result.Success(dto);
    }
}