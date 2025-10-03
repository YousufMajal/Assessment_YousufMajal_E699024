namespace Application.Config;

// Configuration options for business rules and validation limits.
public sealed class BusinessRulesOptions
{
    public const string SectionName = "Config:BusinessRules";

    public decimal MaxWithdrawalAmount { get; set; } = 1000000.00m;

    public decimal MinWithdrawalAmount { get; set; } = 0.01m;
}