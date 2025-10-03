using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

//placeholder for actual ef db context
public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<UserAccount> UserAccount { get; set; }
    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }
}