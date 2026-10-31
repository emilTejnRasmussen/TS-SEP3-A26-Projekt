using DTO.Event;

namespace GrpcClient;

public class EventGrpcClient : IEventGrpcClient
{
    public Task<EventResponse> CreateEventAsync(CreateEventRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<EventResponse> GetEventByIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}