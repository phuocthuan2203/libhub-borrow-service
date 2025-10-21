namespace LibHub.BorrowService.Domain.Entities;

public class Loan
{
    public Guid Id { get; private set; }
    public Guid CopyId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public string Status { get; private set; } = string.Empty;

    private Loan() {}

    public static Loan Create(Guid copyId, Guid customerId)
    {
        return new Loan
        {
            Id = Guid.NewGuid(),
            CopyId = copyId,
            CustomerId = customerId,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = "Active"
        };
    }

    public void MarkAsReturned()
    {
        Status = "Returned";
        ReturnDate = DateTime.UtcNow;
    }
}
