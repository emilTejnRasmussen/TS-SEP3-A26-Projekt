using DTO.Event;

namespace WebApp.Services;

public interface IEventService
{
    Task<EventResponse> CreateAsync(CreateEventRequest request);
    Task<List<EventOverviewResponse>> GetAllAsync();
}
