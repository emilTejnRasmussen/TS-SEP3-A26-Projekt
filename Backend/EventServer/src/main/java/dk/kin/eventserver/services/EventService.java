package dk.kin.eventserver.services;

import dk.kin.eventserver.entities.Event;
import dk.kin.eventserver.repositories.EventRepository;
import jakarta.validation.ConstraintViolation;
import jakarta.validation.ConstraintViolationException;
import jakarta.validation.Validator;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Set;

@Service
public class EventService {

    private final EventRepository eventRepository;
    private final Validator validator;

    public EventService(EventRepository eventRepository, Validator validator) {
        this.eventRepository = eventRepository;
        this.validator = validator;
    }

    public Event createEvent(Event event) {
        Set<ConstraintViolation<Event>> violations = validator.validate(event);

        if (!violations.isEmpty()) {
            throw new ConstraintViolationException(violations);
        }

        if (!event.getDateTime().isAfter(LocalDateTime.now())) {
            throw new IllegalArgumentException("Event date must be in the future");
        }

        return eventRepository.save(event);
    }

    public List<Event> getAllEvents()
    {
        return eventRepository.findAll();
    }
}
