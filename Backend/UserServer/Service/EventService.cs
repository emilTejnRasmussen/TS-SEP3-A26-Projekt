using DTO.Event;
using GrpcClient;
using Service.Interface;

namespace Service;

public class EventService(IEventGrpcClient eventClient) : IEventService
{
    public Task<EventResponse> CreateEventAsync(CreateEventRequest request)
    {
        return eventClient.CreateEventAsync(request);
    }

    public Task<EventResponse> GetEventByIdAsync(long id)
    {
        return eventClient.GetEventByIdAsync(id);
    }

    public Task<List<EventResponse>> GetManyAsync()
    {
        return eventClient.GetManyAsync();
    }
}