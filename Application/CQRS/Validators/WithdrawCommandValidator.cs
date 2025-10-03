using Application.Config;
using Application.CQRS.Commands.Withdraw;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Application.CQRS.Validators;

internal sealed class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawCommandValidator(IOptions<BusinessRulesOptions> businessRulesOptions)
    {
        var options = businessRulesOptions.Value;

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId must is required");

        RuleFor(x => x.Amount)
            .GreaterThan(options.MinWithdrawalAmount).WithMessage($"Withdrawal amount must be greater than {options.MinWithdrawalAmount}")
            .LessThanOrEqualTo(options.MaxWithdrawalAmount).WithMessage($"Withdrawal amount exceeds configured limit of {options.MaxWithdrawalAmount}");
    }
}