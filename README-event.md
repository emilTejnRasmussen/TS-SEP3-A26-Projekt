# EventServer

EventServer is the Java/Spring Boot service responsible for event data in the KIN application. It uses gRPC for communication with UserServer and PostgreSQL for persistence.

This guide explains how to prepare the development environment and run EventServer using either Podman or a locally installed PostgreSQL database.

## Contents

* [Prerequisites](#prerequisites)
* [Option A: Run with Podman](#option-a-run-with-podman)
* [Option B: Run without Podman](#option-b-run-without-podman)
* [Test the gRPC service](#test-the-grpc-service)
* [Troubleshooting](#troubleshooting)

## Prerequisites

The following tools are used to develop, build, run, and test EventServer on Windows.

| Tool            | Check installation       | Expected version                  | Install command (Windows)                       | Notes                                                                                                                                                           |
| --------------- | ------------------------ | --------------------------------- | ----------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| IDE             | —                        | Current version                   | —                                               | IntelliJ IDEA is recommended because it makes running Spring Boot and managing Maven easier. VS Code is also possible with Microsoft's Extension Pack for Java. |
| Java JDK        | `java -version`          | 21.x                              | `winget install EclipseAdoptium.Temurin.21.JDK` | Required to compile and run EventServer. Set the Project SDK to JDK 21 in your IDE.                                                                             |
| Git             | `git --version`          | 2.40+                             | `winget install Git.Git`                        | Used to clone the repository and manage changes.                                                                                                                |
| Maven Wrapper   | `.\mvnw.cmd -version`    | Version configured by the project | Included in the repository                      | A separate global Maven installation is not required.                                                                                                           |
| Podman Desktop  | `podman --version`       | Podman 4+                         | `winget install -e --id RedHat.Podman-Desktop`  | Only required for the Podman setup.                                                                                                                             |
| Podman Compose  | `podman compose version` | Available                         | Included/configured through Podman Desktop      | Only required for the Podman setup.                                                                                                                             |
| PostgreSQL      | `psql --version`         | 17.x                              | See PostgreSQL section below                    | Only required for the non-Podman setup.                                                                                                                         |
| Postman Desktop | —                        | Current version                   | Install from the official Postman website       | Optional. Used to send and inspect gRPC requests.                                                                                                               |

After installing tools, close and reopen PowerShell and restart your IDE if it was already open. This allows them to pick up updated `PATH` variables and environment settings.

### Maven Wrapper

The project includes the Maven Wrapper (`mvnw` and `mvnw.cmd`). You do not need to install Maven separately. The wrapper downloads and uses the Maven version configured by the project.

From the repository root, verify the wrapper:

```powershell
.\Backend\EventServer\mvnw.cmd -version
```

The first invocation may download Maven. A JDK is still required.

If you prefer installing Maven globally and using the `mvn` command, follow the instructions at https://maven.apache.org/install.html.

# Option A: Run with Podman

This setup uses Podman Compose to build and start both PostgreSQL and EventServer in containers.

## 1. Install Podman

Install Podman Desktop:

```powershell
winget install -e --id RedHat.Podman-Desktop
```

Open Podman Desktop and complete its setup. Ensure that Podman Compose is selected/enabled during setup.

Initialise the Podman machine once, if one does not already exist:

```powershell
podman machine init
```

Start the machine:

```powershell
podman machine start
```

Verify Podman and Compose:

```powershell
podman --version
podman compose version
```

You can close the Podman Desktop window afterwards. The Podman machine and Compose commands can be managed from PowerShell.

## 2. Configure environment variables

The project uses environment variables for database configuration.

Create a file named `.env` in the **repository root**, alongside `compose.yaml`.

Copy the following content into the file:

```dotenv
# EventServer database
EVENT_DB_HOST=event-db
EVENT_DB_PORT=5432
EVENT_DB_NAME=kin_events
EVENT_DB_USER=kin_event
EVENT_DB_PASSWORD=kin_dev_password
```

The `.env` file is read by Podman Compose. Compose uses these values to configure the PostgreSQL container and passes the database connection settings to EventServer.

**Important:** The `.env` file contains local development credentials. It must be included in `.gitignore` and must not be committed to Git.

## 3. Build and start the application

Run the following command from the **repository root**, where `compose.yaml` is located:

```powershell
podman compose up -d --build
```

This builds the EventServer image and starts both EventServer and PostgreSQL in the background.

To check which containers are running:

```powershell
podman ps
```

You should see the event-db and the event-server container.

If a container does not start, inspect its logs using the container name shown by `podman ps -a`:

```powershell
podman logs <container-name>
```

## 4. Verify the database and application

EventServer connects to PostgreSQL through the Compose network using the database service name and the container port.

The gRPC endpoint is exposed on your local machine at:

```text
localhost:9090
```

Continue to [Test the gRPC service](#test-the-grpc-service) to verify the application.

## 5. Stop the application

To stop the containers:

```powershell
podman compose down
```

This stops and removes the containers and Compose network but normally preserves named volumes and database data.

To deliberately delete the database volume and its data as well:

```powershell
podman compose down -v
```

**Warning:** `down -v` permanently removes the Compose-managed database volume. Do not use it for an ordinary restart.

When you run `podman compose up -d` again, Compose automatically creates a new, empty volume and initializes the database.

# Option B: Run without Podman

In this setup, PostgreSQL and EventServer run directly on your computer. Podman is not needed.

## 1. Install PostgreSQL

Install PostgreSQL 17 using the official PostgreSQL Windows installer.

During installation, note the password you set for the PostgreSQL administrator and the port (normally `5432`).

Verify the command-line client:

```powershell
psql --version
```

Make sure the PostgreSQL Windows service is running before continuing.

## 2. Create the database and application user

Open SQL Shell (`psql`) or another PostgreSQL client and connect as the PostgreSQL administrator.

Create a database user and database:

```sql
CREATE USER kin_event WITH PASSWORD 'kin_dev_password';
CREATE DATABASE kin_events OWNER kin_event;
```

Use a local development password only. Do not use a real or shared password in source control.

If the database or user already exists, do not run the corresponding `CREATE` command again.

You can connect to the database with:

```powershell
psql -h localhost -p 5432 -U kin_event -d kin_events
```

## 3. Configure environment variables

The project uses a `.env` file for database configuration.

Create or update the `.env` file in the **repository root**, alongside `compose.yaml`.

For this setup, use the following values:

```dotenv
# EventServer database
EVENT_DB_HOST=localhost
EVENT_DB_PORT=5432
EVENT_DB_NAME=kin_events
EVENT_DB_USER=kin_event
EVENT_DB_PASSWORD=kin_dev_password
```

The host and port point to your locally installed PostgreSQL instance. If you have other values for name, user and password use those instead.

**Important:** The `.env` file contains local development credentials. It must be included in `.gitignore` and must not be committed to Git.

### application.properties

The `application.properties` file already imports the `.env` file:

```properties
spring.config.import=optional:file:../../.env[.properties]

spring.application.name=EventServer

spring.datasource.url=jdbc:postgresql://${EVENT_DB_HOST:localhost}:${EVENT_DB_PORT}/${EVENT_DB_NAME}
spring.datasource.username=${EVENT_DB_USER}
spring.datasource.password=${EVENT_DB_PASSWORD}

spring.jpa.hibernate.ddl-auto=update
```

Spring Boot loads the environment variables from `.env` when EventServer starts.

The import path assumes that the application's working directory is `Backend/EventServer`. If you run EventServer from IntelliJ, make sure the **Working directory** in the Run Configuration is set to that folder.

## 4. Build and run EventServer

Open PowerShell in the repository root and run:

```powershell
cd Backend\EventServer
.\mvnw.cmd clean compile
```

For ordinary Java changes, `compile` is normally sufficient:

```powershell
.\mvnw.cmd compile
```

Use `clean` when generated Protobuf/gRPC code or build output may be stale, or after changing the Maven configuration.

Start the application with Maven:

```powershell
.\mvnw.cmd spring-boot:run
```

Alternatively, open `Backend/EventServer` as a Maven project in IntelliJ and run `EventServerApplication`.

When startup succeeds, the application should connect to your local PostgreSQL database and start its gRPC server.

The local gRPC address is:

```text
localhost:9090
```

Keep the terminal running while testing. Press `Ctrl+C` to stop the Maven-launched application.

Continue to [Test the gRPC service](#test-the-grpc-service).


# Test the gRPC service

Use Postman Desktop or another gRPC client.

1. Start EventServer and make sure PostgreSQL is running.
2. Create a new **gRPC** request in Postman.
3. Set the server address to `localhost:9090`.
4. Import the shared contract file: `Contracts/proto/event.proto`.
5. Select the `kin.events.EventService` service.
6. Select `GetAllEvents` and send an empty request:

```json
{}
```

An empty `events` array is a valid response when the database contains no events.

## Test creating an event

Select `CreateEvent` and send a request such as:

```json
{
  "title": "Test event",
  "short_description": "A test event",
  "long_description": "A test event created from Postman",
  "location": "Aarhus",
  "max_participants": 20,
  "date_time": "2027-06-15T18:00:00",
  "host_ids": [1]
}
```

The response should contain the event and its database-generated ID.

Call `GetAllEvents` again to verify that the event can be retrieved.

## Test validation

For example, set `max_participants` to `0`.

The service should reject the request with gRPC status `INVALID_ARGUMENT`, because the field must be positive.

The current `host_ids` values are treated as user IDs. At this stage, EventServer does not yet verify those IDs against UserServer.

# Troubleshooting

| Problem                                     | Things to check                                                                                                                        |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `java` is not recognised                    | Reopen PowerShell; check JDK 21 installation and `PATH`.                                                                               |
| Maven Wrapper cannot run                    | Confirm that `mvnw.cmd` exists in `Backend\EventServer`; check that Java is installed.                                                 |
| EventServer cannot connect to PostgreSQL    | Confirm PostgreSQL is running and that the JDBC host, port, database, username and password match the selected setup.                  |
| Connection refused on port `5433`           | For the Podman option, check the published port in `compose.yaml` and verify the database container is running.                        |
| Connection refused on port `5432`           | For the non-Podman option, check that the local PostgreSQL Windows service is running and listening on port `5432`.                    |
| gRPC request cannot connect                 | Confirm EventServer started successfully and check that the gRPC server is listening on port `9090`.                                   |
| Port is already in use                      | Check whether another PostgreSQL or EventServer process is already using the configured port.                                          |
| Protobuf/gRPC generated classes are missing | Run `.\mvnw.cmd clean compile` from `Backend\EventServer`; verify that the shared proto path in `pom.xml` points to `Contracts\proto`. |

## Useful commands

```powershell
# Show running Podman containers
podman ps

# Show all containers, including stopped ones
podman ps -a

# Show database container logs
podman logs <container-name>

# Rebuild generated and compiled Java sources
.\mvnw.cmd clean compile

# Run EventServer locally
.\mvnw.cmd spring-boot:run
```

---

This README describes the current EventServer walking skeleton. Update the instructions when ports, database settings, Compose service names, or the shared Protobuf contract change.
