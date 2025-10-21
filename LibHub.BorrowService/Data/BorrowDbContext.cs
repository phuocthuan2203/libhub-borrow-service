using LibHub.BorrowService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibHub.BorrowService.Data;

public class BorrowDbContext : DbContext
{
    public BorrowDbContext(DbContextOptions<BorrowDbContext> options) : base(options)
    {
    }

    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("loans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CopyId).HasColumnName("copy_id").IsRequired();
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired();
            entity.Property(e => e.BorrowDate).HasColumnName("borrow_date").IsRequired();
            entity.Property(e => e.DueDate).HasColumnName("due_date").IsRequired();
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();

            entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_loans_customer_id");
        });
    }
}
