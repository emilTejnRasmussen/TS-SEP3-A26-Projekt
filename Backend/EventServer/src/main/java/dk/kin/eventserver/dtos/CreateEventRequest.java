package dk.kin.eventserver.dtos;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.Positive;

import java.time.LocalDateTime;
import java.util.Set;

public record CreateEventRequest (
        @NotBlank String title,
        @NotBlank String shortDescription,
        String longDescription,
        @NotBlank String location,
        @Positive int maxParticipants,
        @NotBlank LocalDateTime dateTime,
        @NotEmpty Set<Long> hostIds
)
{}