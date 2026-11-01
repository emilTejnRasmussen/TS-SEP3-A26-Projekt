package dk.kin.eventserver.grpc;

import dk.kin.eventserver.services.EventService;
import dk.kin.eventserver.entities.Event;
import io.grpc.stub.StreamObserver;
import io.grpc.Status;
import jakarta.validation.ConstraintViolationException;
import org.springframework.grpc.server.service.GrpcService;

import java.time.LocalDateTime;
import java.time.format.DateTimeParseException;
import java.util.HashSet;
import java.util.List;
import java.util.stream.Collectors;

@GrpcService
public class EventGrpcService extends EventServiceGrpc.EventServiceImplBase {

    private final EventService eventService;

    public EventGrpcService(EventService eventService) {
        this.eventService = eventService;
    }

    @Override
    public void createEvent(
            CreateEventRequest request,
            StreamObserver<EventResponse> responseObserver) {
        try {
            Event event = new Event();

            event.setTitle(request.getTitle());
            event.setShortDescription(request.getShortDescription());
            event.setLongDescription(request.getLongDescription());
            event.setLocation(request.getLocation());
            event.setMaxParticipants(request.getMaxParticipants());
            event.setDateTime(LocalDateTime.parse(request.getDateTime()));
            event.setHostIds(new HashSet<>(request.getHostIdsList()));

            Event created = eventService.createEvent(event);

            EventResponse eventResponse = EventResponse.newBuilder()
                    .setId(created.getId())
                    .setTitle(created.getTitle())
                    .setShortDescription(created.getShortDescription())
                    .setLongDescription(created.getLongDescription())
                    .setLocation(created.getLocation())
                    .setMaxParticipants(created.getMaxParticipants())
                    .setDateTime(created.getDateTime().toString())
                    .addAllHostIds(created.getHostIds())
                    .build();

            responseObserver.onNext(eventResponse);
            responseObserver.onCompleted();

        } catch (DateTimeParseException e) {
            responseObserver.onError(
                    Status.INVALID_ARGUMENT
                            .withDescription("Invalid date format")
                            .asRuntimeException()
            );

        } catch (ConstraintViolationException e) {
            String errors = e.getConstraintViolations().stream()
                    .map(violation ->
                            violation.getPropertyPath()
                                    + ": " + violation.getMessage())
                    .collect(Collectors.joining("; "));

            responseObserver.onError(
                    Status.INVALID_ARGUMENT
                            .withDescription(errors)
                            .asRuntimeException()
            );

        } catch (IllegalArgumentException e) {
            responseObserver.onError(
                    Status.INVALID_ARGUMENT
                            .withDescription(e.getMessage())
                            .asRuntimeException()
            );

        } catch (IllegalStateException e) {
            responseObserver.onError(
                    Status.FAILED_PRECONDITION
                            .withDescription(e.getMessage())
                            .asRuntimeException()
            );

        } catch (Exception e) {
            responseObserver.onError(
                    Status.INTERNAL
                            .withDescription("An unexpected error occurred")
                            .asRuntimeException()
            );
        }
    }

    @Override
    public void getAllEvents(
            Empty request,
            StreamObserver<EventListResponse> responseObserver) {
        try {
            List<Event> events = eventService.getAllEvents();

            EventListResponse.Builder eventListResponseBuilder = EventListResponse.newBuilder();

            for (Event event : events) {
                EventOverviewResponse eventOverviewResponse = EventOverviewResponse.newBuilder()
                        .setId(event.getId())
                        .setTitle(event.getTitle())
                        .setShortDescription(event.getShortDescription())
                        .setLocation(event.getLocation())
                        .setDateTime(event.getDateTime().toString())
                        .build();

                eventListResponseBuilder.addEvents(eventOverviewResponse);
            }

            responseObserver.onNext(eventListResponseBuilder.build());
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(
                    Status.INTERNAL
                            .withDescription("An unexpected error occurred")
                            .asRuntimeException()
            );
        }
    }
}
