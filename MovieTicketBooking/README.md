# Movie Ticket Booking System

## 1. Project Overview

The Movie Ticket Booking System is a backend application developed using ASP.NET Core Web API and Entity Framework Core. It allows users to manage movies, theatres, screens, seats, shows, and movie ticket bookings.

The project follows a layered architecture to maintain separation of responsibilities and make the application easier to maintain and extend.

## 2. Technologies Used

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Programming Language:** C#
- **ORM:** Entity Framework Core 10
- **Database:** Microsoft SQL Server
- **API Testing:** Postman
- **IDE:** Visual Studio
- **Version Control:** Git and GitHub

## 3. Project Architecture

The application follows the Controller → Service → Repository → DbContext architecture.

- **Controllers:** Handle HTTP requests and responses.
- **Services:** Implement application business logic.
- **Repositories:** Handle database operations.
- **DbContext:** Manages communication between the application and SQL Server.
- **Models:** Represent database entities.
- **DTOs:** Transfer data between API requests and responses.

## 4. Main Modules

1. Movie Management
2. Theatre Management
3. Screen Management
4. Seat Management
5. Show Management
6. Show Seat Management
7. Booking Management

## 5. Implementation Progress

| Module | Status |
|---|---|
| Movie Management | Completed |
| Theatre Management | Pending |
| Screen Management | Pending |
| Seat Management | Pending |
| Show Management | Pending |
| Show Seat Management | Pending |
| Booking Management | Pending |

## 6. Movie Management API

The Movie Management module supports the following operations:

| Operation | HTTP Method | Endpoint |
|---|---|---|
| Create Movie | POST | `/api/movies` |
| Get All Movies | GET | `/api/movies` |
| Get Movie By ID | GET | `/api/movies/{id}` |
| Update Movie | PUT | `/api/movies/{id}` |
| Delete Movie | DELETE | `/api/movies/{id}` |
| Search Movie By Title | GET | Movie search endpoint configured in the controller |

## 7. Database

Database Name: `MovieTicketBookingDB`

Database connection is configured in `appsettings.json`.

Entity Framework Core migrations are used to create and update database tables.

## 8. Running the Project

1. Clone the repository.
2. Open the project in Visual Studio.
3. Update the SQL Server connection string in `appsettings.json` if required.
4. Open the terminal in the project directory.
5. Restore dependencies:

   `dotnet restore`

6. Apply database migrations:

   `dotnet ef database update`

7. Run the application:

   `dotnet run`

8. Test the API endpoints using Postman.

## 9. API Testing

Postman is used to test the API endpoints.

The Postman collection contains requests for creating, retrieving, updating, deleting, and searching movies.

## 10. Future Enhancements

- Complete Theatre, Screen, Seat, and Show management.
- Implement automatic show-seat generation.
- Implement ticket booking and cancellation.
- Add transaction handling for booking operations.
- Develop an Angular frontend.