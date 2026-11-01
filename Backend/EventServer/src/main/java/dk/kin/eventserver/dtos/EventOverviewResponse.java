package dk.kin.eventserver.dtos;

import java.time.LocalDateTime;

public record EventOverviewResponse(
        Long id,
        String title,
        String shortDescription,
        String location,
        LocalDateTime dateTime
)
{}
