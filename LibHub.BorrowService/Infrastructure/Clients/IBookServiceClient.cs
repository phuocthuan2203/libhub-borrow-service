namespace LibHub.BorrowService.Infrastructure.Clients;

public interface IBookServiceClient
{
    Task<string?> GetCopyStatusAsync(Guid copyId);
    Task<bool> UpdateCopyStatusAsync(Guid copyId, string newStatus);
}
