namespace Domain.Entities;

public class UserAccount
{
    public Guid accountId { get; set; }
    public decimal balance { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}