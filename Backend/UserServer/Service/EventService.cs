using DTO.Event;
using Service.Interface;

namespace Service;

public class EventService : IEventService
{
    public Task<EventResponse> CreateEventAsync(CreateEventRequest request)
    {
        // TODO: Forward request to Java EventServer via gRPC
        return Task.FromResult(new EventResponse());
    }

    public Task<EventResponse> GetEventByIdAsync(long id)
    {
        // TODO: Retrieve event from Java EventServer via gRPC
        return Task.FromResult(new EventResponse());
    }
}