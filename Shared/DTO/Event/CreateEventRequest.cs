namespace DTO.Event;

public record CreateEventRequest(
    string Title,
    string ShortDescription,
    string LongDescription,
    string Location,
    int MaxParticipants,
    DateTime DateTime,
    List<long> HostIds
);