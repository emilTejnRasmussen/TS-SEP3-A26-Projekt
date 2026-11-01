namespace WebApp.Client.DTO.Event;

// Felterne følger Contracts/proto/event.proto (EventResponse).
// TODO: Tjek mod backendens Shared/DTO/Event/EventResponse, når den er udfyldt.
public record EventResponse(
    long Id,
    string Title,
    string ShortDescription,
    string? LongDescription,
    string Location,
    int MaxParticipants,
    DateTime DateTime,
    List<long> HostIds
);
