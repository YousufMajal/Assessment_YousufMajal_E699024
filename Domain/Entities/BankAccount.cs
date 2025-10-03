namespace Domain.Entities;

public record BankAccount
{
    public Guid AccountId { get; set; }
    public decimal Balance { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}