# RTU Schedule Analyzer

RTU Schedule Analyzer - it's a service for detecting inconveniences in the schedule of RTU MIREA.

## Clean Architecture & Design Patterns
This project follows the principles of Clean Architecture, ensuring a maintainable codebase. It separates concerns into distinct layers:

- **Core Layer**: Contains business logic and domain entities.
- **Infrastructure Layer**: Responsible for data persistence and external integrations.
- **Presentation Layer (API)**: Exposes endpoints for external communication.

The project also utilizes design patterns such as:
- **Dependency Injection (DI)** for better modularity.
- **Repository Pattern** for data abstraction.

## Technologies Used
- **ASP.NET Core** for the web API.
- **Entity Framework Core** for data access.
- **Postgre SQL** as the database.
- **Docker and Docker Compose** for containerization.
- **xUnit** for unit testing.
- **Swagger** for API documentation.
