using Application.Abstractions;
using Application.DTOs;
using Application.Errors;
using Application.Events;
using Application.Interfaces;
using Application.Interfaces.Repositories;

namespace Application.CQRS.Commands.Withdraw;

internal sealed class WithdrawFundsCommandHandler : ICommandHandler<WithdrawFundsCommand, WithdrawalResultDto>
{
    private readonly IAccountRepository _accounts;
    private readonly IOutboxWriter _outbox;
    private static readonly Error AccountNotFound = AccountErrorResponses.AccountNotFoundError();
    private static readonly Error InsufficientFunds = AccountErrorResponses.InsufficientFundsError();

    public WithdrawFundsCommandHandler(
        IAccountRepository accounts,
        IOutboxWriter outbox)
    {
        _accounts = accounts;
        _outbox = outbox;
    }

    public async Task<Result<WithdrawalResultDto>> Handle(WithdrawFundsCommand command, CancellationToken cancellationToken)
    {
        // Check if account exists & get balance
        var account = await _accounts.GetByIdAsync(command.AccountId);
        if (account is null)
        {
            return Result.Failure<WithdrawalResultDto>(AccountNotFound);
        }
        var previous = account.balance;

        if (previous < command.Amount)
        {
            return Result.Failure<WithdrawalResultDto>(InsufficientFunds);
        }

        account.balance -= command.Amount;
        _accounts.Update(account);

        var integrationEvent = new WithdrawalPerformedIntegrationEvent(
            EventId: Guid.NewGuid(),
            AccountId: command.AccountId,
            Amount: command.Amount,
            PreviousBalance: previous,
            NewBalance: account.balance,
            OccurredUtc: DateTime.UtcNow);

        _outbox.Enqueue(integrationEvent, "banking.withdrawal.performed.v1");

        var dto = new WithdrawalResultDto(
            AccountId: command.AccountId,
            PreviousBalance: previous,
            WithdrawnAmount: command.Amount,
            NewBalance: account.balance,
            EventStatus: "ENQUEUED",
            TimestampUtc: DateTime.UtcNow);

        return Result.Success(dto);
    }
}