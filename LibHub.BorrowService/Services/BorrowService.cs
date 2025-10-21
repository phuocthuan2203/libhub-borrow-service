using LibHub.BorrowService.Data.Repositories;
using LibHub.BorrowService.Domain.Entities;
using LibHub.BorrowService.DTOs;
using LibHub.BorrowService.Infrastructure.Clients;

namespace LibHub.BorrowService.Services;

public class BorrowService : IBorrowService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookServiceClient _bookServiceClient;
    private readonly ILogger<BorrowService> _logger;
    private const int MaxBorrowLimit = 5;

    public BorrowService(
        ILoanRepository loanRepository, 
        IBookServiceClient bookServiceClient,
        ILogger<BorrowService> logger)
    {
        _loanRepository = loanRepository;
        _bookServiceClient = bookServiceClient;
        _logger = logger;
    }

    public async Task<LoanDto?> BorrowBookAsync(Guid copyId, Guid customerId)
    {
        var activeLoans = await _loanRepository.GetActiveLoansByCustomerAsync(customerId);
        if (activeLoans.Count >= MaxBorrowLimit)
        {
            _logger.LogWarning("Customer {CustomerId} has reached the borrowing limit", customerId);
            return null;
        }

        var copyStatus = await _bookServiceClient.GetCopyStatusAsync(copyId);
        if (string.IsNullOrEmpty(copyStatus) || copyStatus != "Available")
        {
            _logger.LogWarning("Copy {CopyId} is not available. Status: {Status}", copyId, copyStatus);
            return null;
        }

        var newLoan = Loan.Create(copyId, customerId);
        var savedLoan = await _loanRepository.AddAsync(newLoan);

        var updateSuccess = await _bookServiceClient.UpdateCopyStatusAsync(copyId, "On Loan");
        if (!updateSuccess)
        {
            _logger.LogError("Failed to update copy {CopyId} status to 'On Loan'. Loan {LoanId} created but copy status not updated.", 
                copyId, savedLoan.Id);
        }

        return MapToDto(savedLoan);
    }

    public async Task<bool> ReturnBookAsync(Guid loanId, Guid customerId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null)
        {
            _logger.LogWarning("Loan {LoanId} not found", loanId);
            return false;
        }

        if (loan.CustomerId != customerId)
        {
            _logger.LogWarning("Loan {LoanId} does not belong to customer {CustomerId}", loanId, customerId);
            return false;
        }

        if (loan.Status != "Active")
        {
            _logger.LogWarning("Loan {LoanId} is not active. Status: {Status}", loanId, loan.Status);
            return false;
        }

        loan.MarkAsReturned();
        await _loanRepository.SaveChangesAsync();

        var updateSuccess = await _bookServiceClient.UpdateCopyStatusAsync(loan.CopyId, "Available");
        if (!updateSuccess)
        {
            _logger.LogError("Failed to update copy {CopyId} status to 'Available'. Loan {LoanId} marked as returned but copy status not updated.", 
                loan.CopyId, loanId);
        }

        return true;
    }

    public async Task<List<LoanDto>> GetCustomerLoansAsync(Guid customerId)
    {
        var loans = await _loanRepository.GetActiveLoansByCustomerAsync(customerId);
        return loans.Select(MapToDto).ToList();
    }

    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            CopyId = loan.CopyId,
            CustomerId = loan.CustomerId,
            BorrowDate = loan.BorrowDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status
        };
    }
}
