using LibHub.BorrowService.DTOs;

namespace LibHub.BorrowService.Services;

public interface IBorrowService
{
    Task<LoanDto?> BorrowBookAsync(Guid copyId, Guid customerId);
    Task<bool> ReturnBookAsync(Guid loanId, Guid customerId);
    Task<List<LoanDto>> GetCustomerLoansAsync(Guid customerId);
}
