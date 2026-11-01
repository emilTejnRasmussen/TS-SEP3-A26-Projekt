namespace DTO.Event;

public record EventResponse(
    long Id,
    string Title,
    string ShortDescription,
    string LongDescription,
    string Location,
    int MaxParticipants,
    DateTime DateTime,
    List<long> HostIds,
    List<HostResponse>? Hosts = null
);