namespace Infrastructure.Aws.Models;

// Configuration options for AWS integration.
public sealed class AwsOptions
{
    public const string SectionName = "Config:Aws";
    public string Region { get; set; } = string.Empty;
    public string? AccessKeyId { get; set; }
    public string? SecretAccessKey { get; set; }
    public string? SessionToken { get; set; }
    public string WithdrawalTopicArn { get; set; } = string.Empty;
}