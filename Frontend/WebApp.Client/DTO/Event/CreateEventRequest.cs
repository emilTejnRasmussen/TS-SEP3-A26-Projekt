using System.ComponentModel.DataAnnotations;

namespace WebApp.Client.DTO.Event;

// Felterne følger Contracts/proto/event.proto (CreateEventRequest).
// TODO: Tjek mod backendens Shared/DTO/Event/CreateEventRequest, når den er udfyldt.
public class CreateEventRequest
{
    [Required(ErrorMessage = "Titel er påkrævet")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Kort beskrivelse er påkrævet")]
    public string ShortDescription { get; set; } = "";

    public string? LongDescription { get; set; }

    [Required(ErrorMessage = "Placering er påkrævet")]
    public string Location { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Max deltagere skal være mindst 1")]
    public int MaxParticipants { get; set; } = 10;

    // Uden tidszone (Kind = Unspecified), fordi Event Serveren læser den som Java LocalDateTime.
    public DateTime DateTime { get; set; } =
        DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(18), DateTimeKind.Unspecified);

    // Et event kan have flere værter. Siden sætter den indloggede bruger som vært.
    // TODO: Fjern, når backenden selv tager værten fra JWT'en.
    public List<long> HostIds { get; set; } = [];
}
