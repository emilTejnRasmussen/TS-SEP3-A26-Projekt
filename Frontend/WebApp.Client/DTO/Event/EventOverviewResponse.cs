namespace WebApp.Client.DTO.Event;

// Det korte format, som listen over events returnerer.
// Følger Contracts/proto/event.proto (EventOverviewResponse i GetAllEvents).
public record EventOverviewResponse(
    long Id,
    string Title,
    string ShortDescription,
    string Location,
    DateTime DateTime
);
