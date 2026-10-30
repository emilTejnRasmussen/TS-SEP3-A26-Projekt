using System.Net.Http.Json;
using WebApp.Client.DTO.Event;

namespace WebApp.Client.Services;

public class HttpEventService(HttpClient client) : IEventService
{
    // POST /Events
    public async Task<EventResponse> CreateAsync(CreateEventRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiEndpoints.Events, request);
        await ApiException.ThrowIfFailedAsync(response);
        return (await response.Content.ReadFromJsonAsync<EventResponse>())!;
    }

    // GET /Events
    public async Task<List<EventResponse>> GetAllAsync()
    {
        var response = await client.GetAsync(ApiEndpoints.Events);
        await ApiException.ThrowIfFailedAsync(response);
        return await response.Content.ReadFromJsonAsync<List<EventResponse>>() ?? [];
    }
}
