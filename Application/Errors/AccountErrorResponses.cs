using Application.Abstractions;

namespace Application.Errors;

public static class AccountErrorResponses
{
    public static Error AccountNotFoundError() => new(
        "Account.NotFound",
        $"Account not found");

    public static Error AccountNotFoundError(Guid id) => new(
        "Account.NotFound",
        $"Account not found for {id}");

    public static Error InsufficientFundsError() => new(
        "Account.InsufficientFunds",
        $"Insufficient funds for account");
}