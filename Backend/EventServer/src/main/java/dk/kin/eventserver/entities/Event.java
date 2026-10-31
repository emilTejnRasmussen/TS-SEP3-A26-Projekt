package dk.kin.eventserver.entities;

import jakarta.persistence.*;
import jakarta.validation.constraints.Positive;

import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "events")
public class Event {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String title;

    @Column(nullable = false)
    private String shortDescription;

    private String longDescription;

    @Column(nullable = false)
    private String location;

    @Positive
    private int maxParticipants;

    @Column(nullable = false)
    private LocalDateTime dateTime;

    @ElementCollection
    @CollectionTable(
            name = "event_hosts",
            joinColumns = @JoinColumn(name = "event_id", nullable = false))
    @Column(name = "user_id", nullable = false)
    private Set<Long> hostIds = new HashSet<>();
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

}
