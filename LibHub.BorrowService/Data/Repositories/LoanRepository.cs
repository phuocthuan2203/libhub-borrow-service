using LibHub.BorrowService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibHub.BorrowService.Data.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly BorrowDbContext _context;

    public LoanRepository(BorrowDbContext context)
    {
        _context = context;
    }

    public async Task<Loan> AddAsync(Loan loan)
    {
        await _context.Loans.AddAsync(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan?> GetByIdAsync(Guid id)
    {
        return await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Loan>> GetActiveLoansByCustomerAsync(Guid customerId)
    {
        return await _context.Loans
            .Where(l => l.CustomerId == customerId && l.Status == "Active")
            .ToListAsync();
    }

    public async Task<Loan?> GetActiveLoanByCopyIdAsync(Guid copyId)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.CopyId == copyId && l.Status == "Active");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
