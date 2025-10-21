using System.Text;
using System.Text.Json;

namespace LibHub.BorrowService.Infrastructure.Clients;

public class BookServiceClient : IBookServiceClient
{
    private readonly HttpClient _httpClient;

    public BookServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetCopyStatusAsync(Guid copyId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/books/copies/{copyId}/status");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var statusObject = JsonSerializer.Deserialize<JsonElement>(content);
                return statusObject.GetProperty("status").GetString();
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> UpdateCopyStatusAsync(Guid copyId, string newStatus)
    {
        try
        {
            var requestBody = new { status = newStatus };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/books/copies/{copyId}/status", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
