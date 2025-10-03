using Application.Abstractions;

namespace Application.Errors;

public static class AccountErrorResponses
{
    public static Error AccountNotFoundError() => new(
        "Account.Error",
        $"Account not found");

    public static Error AccountNotFoundError(Guid id) => new(
        "Account.Error",
        $"Account not found for {id}");

    public static Error InsufficientFundsError() => new(
        "Account.Error",
        $"Insufficient funds for account");
}