namespace LibHub.BorrowService.DTOs;

public class LoanDto
{
    public Guid Id { get; set; }
    public Guid CopyId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
