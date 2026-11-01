package dk.kin.eventserver.entities;

import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Positive;
import jakarta.validation.constraints.Size;

import java.time.LocalDateTime;
import java.util.Collections;
import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "events")
public class Event {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank
    @Column(nullable = false)
    private String title;

    @NotBlank
    @Column(nullable = false)
    private String shortDescription;

    private String longDescription;

    @NotBlank
    @Column(nullable = false)
    private String location;

    @Positive
    private int maxParticipants;

    @NotNull
    @Column(nullable = false)
    private LocalDateTime dateTime;

    @ElementCollection
    @NotNull
    @Size(min = 1)
    @CollectionTable(
            name = "event_hosts",
            joinColumns = @JoinColumn(name = "event_id", nullable = false))
    @Column(name = "user_id", nullable = false)
    private Set<@NotNull Long> hostIds = new HashSet<>();
    /*
    Fordi den ene entitet tilhører en anden database kan vi ikke lave en @ManyToMany joint table.
    I stedet gemmer vi user_id som Long-værdier i en separat tabel koblet sammen med Event entiteten.

    Et alternativ er at lave en separat EventHost entitet,
    især hvis den skal indeholde flere attributter.
    Så vil vi kunne bruge @ManyToOne mellem EventHost og Event.

    Valgt user_id fordi det er users, der er hosts.
    user_id er non-nullable fordi at attributten i join tabellen ikke må være null.

    Fordi hostIds er en liste/set, er det validering i servicelaget,
    der skal sikre, at der altid er mindst en host koblet på et event.
     */

    // GETTERS & SETTERS


    public Long getId() {
        return id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getShortDescription() {
        return shortDescription;
    }

    public void setShortDescription(String shortDescription) {
        this.shortDescription = shortDescription;
    }

    public String getLongDescription() {
        return longDescription;
    }

    public void setLongDescription(String longDescription) {
        this.longDescription = longDescription;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public int getMaxParticipants() {
        return maxParticipants;
    }

    public void setMaxParticipants(int maxParticipants) {
        this.maxParticipants = maxParticipants;
    }

    public LocalDateTime getDateTime() {
        return dateTime;
    }

    public void setDateTime(LocalDateTime dateTime) {
        this.dateTime = dateTime;
    }

    public Set<Long> getHostIds() {
        return Collections.unmodifiableSet(hostIds);
    }

    public void setHostIds(Set<Long> hostIds) {
        if (hostIds == null || hostIds.isEmpty()) {
            throw new IllegalArgumentException("An event must have at least one host");
        }

        if (hostIds.contains(null)) {
            throw new IllegalArgumentException("Host ID cannot be null");
        }

        this.hostIds = new HashSet<>(hostIds);
    }

    public void addHost(Long userId) {
        if (userId == null) {
            throw new IllegalArgumentException("Host ID cannot be null");
        }

        hostIds.add(userId);
    }

    public void removeHost(Long userId) {
        if (userId == null) {
            throw new IllegalArgumentException("Host ID cannot be null");
        }

        if (!hostIds.contains(userId)) {
            throw new IllegalArgumentException("Host is not associated with this event");
        }

        if (hostIds.size() <= 1) {
            throw new IllegalStateException("An event must have at least one host");
        }

        hostIds.remove(userId);
    }
}
