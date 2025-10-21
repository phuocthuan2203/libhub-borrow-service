using LibHub.BorrowService.DTOs;
using LibHub.BorrowService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibHub.BorrowService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowController : ControllerBase
{
    private readonly IBorrowService _borrowService;
    private readonly ILogger<BorrowController> _logger;

    public BorrowController(IBorrowService borrowService, ILogger<BorrowController> logger)
    {
        _borrowService = borrowService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> BorrowBook([FromBody] BorrowRequest request)
    {
        var customerId = GetCustomerIdFromClaims();

        _logger.LogInformation("Borrow request for CopyId: {CopyId} by CustomerId: {CustomerId}", 
            request.CopyId, customerId);

        var loan = await _borrowService.BorrowBookAsync(request.CopyId, customerId);
        
        if (loan == null)
        {
            return BadRequest(new { message = "Unable to borrow book. It may not be available or you have reached your borrowing limit." });
        }

        return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
    }

    [HttpPost("return")]
    public async Task<IActionResult> ReturnBook([FromBody] ReturnRequest request)
    {
        var customerId = GetCustomerIdFromClaims();

        _logger.LogInformation("Return request for LoanId: {LoanId} by CustomerId: {CustomerId}", 
            request.LoanId, customerId);

        var success = await _borrowService.ReturnBookAsync(request.LoanId, customerId);
        
        if (!success)
        {
            return BadRequest(new { message = "Unable to return book. Loan not found or does not belong to you." });
        }

        return Ok(new { message = "Book returned successfully." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoan(Guid id)
    {
        var customerId = GetCustomerIdFromClaims();
        var loans = await _borrowService.GetCustomerLoansAsync(customerId);
        var loan = loans.FirstOrDefault(l => l.Id == id);
        
        if (loan == null)
        {
            return NotFound();
        }

        return Ok(loan);
    }

    [HttpGet("my-loans")]
    public async Task<IActionResult> GetMyLoans()
    {
        var customerId = GetCustomerIdFromClaims();
        var loans = await _borrowService.GetCustomerLoansAsync(customerId);
        return Ok(loans);
    }

    private Guid GetCustomerIdFromClaims()
    {
        var customerIdClaim = User.FindFirst("sub") ?? User.FindFirst("customerId");
        if (customerIdClaim != null && Guid.TryParse(customerIdClaim.Value, out var customerId))
        {
            return customerId;
        }
        
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }
}
