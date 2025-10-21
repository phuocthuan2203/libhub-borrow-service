using LibHub.BorrowService.Domain.Entities;

namespace LibHub.BorrowService.Data.Repositories;

public interface ILoanRepository
{
    Task<Loan> AddAsync(Loan loan);
    Task<Loan?> GetByIdAsync(Guid id);
    Task<List<Loan>> GetActiveLoansByCustomerAsync(Guid customerId);
    Task<Loan?> GetActiveLoanByCopyIdAsync(Guid copyId);
    Task SaveChangesAsync();
}
