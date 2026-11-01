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

## Implementation Progress

| Module | Status |
|---|---|
| Movie Management | Completed |
| Theatre Management | Completed |
| Screen Management | Completed |
| Seat Management | Pending |
| Show Management | Pending |
| Show Seat Management | Pending |
| Booking Management | Pending |

## Screen Management API

Base URL: /api/screens

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/screens | Create screen |
| GET | /api/screens | Get all screens |
| GET | /api/screens?theatreId=1 | Filter screens by theatre |
| GET | /api/screens/{id} | Get screen by ID |
| GET | /api/screens/theatre/{theatreId} | Get screens in a theatre |
| PUT | /api/screens/{id} | Update screen |
| DELETE | /api/screens/{id} | Delete screen |

### Screen Table

| Column | Type | Constraint |
|---|---|---|
| ScreenId | int | Primary Key, Identity |
| TheatreId | int | Foreign Key |
| ScreenName | nvarchar(50) | Required |

### Screen Business Rules

- Every screen must belong to an existing theatre.
- Screen names must be unique within a theatre.
- A theatre can contain multiple screens.
- A screen cannot be deleted when dependent records
- exist
- ## Implementation Progress

| Module | Status |
|---|---|
| Movie Management | Completed |
| Theatre Management | Completed |
| Screen Management | Completed |
| Seat Management | Completed |
| Show Management | Pending |
| Show Seat Management | Pending |
| Booking Management | Pending |
 ## Seat Management API

Base URL: /api/seats

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/seats | Create one seat |
| POST | /api/seats/bulk | Create multiple seats |
| GET | /api/seats/screen/{screenId} | Get seats for a screen |
| GET | /api/seats/{id} | Get seat by ID |
| PUT | /api/seats/{id} | Update seat |
| DELETE | /api/seats/{id} | Delete seat |

### Seats Table

| Column | Type | Constraint |
|---|---|---|
| SeatId | int | Primary Key, Identity |
| ScreenId | int | Foreign Key |
| RowName | nvarchar(5) | Required |
| SeatNumber | int | Required, Greater than 0 |
| SeatType | nvarchar(30) | Required |

### Seat Business Rules

- Every seat must belong to an existing screen.
- Seat number must be greater than zero.
- A seat position must be unique within a screen.
- Duplicate positions are checked during individual and bulk creation.
- Bulk creation must validate all requested positions before saving.
- A screen cannot be deleted while it contains seats.
- A seat cannot be deleted when dependent show or booking records exist.
- ## ShowSeats Management Module

### Features

- View the complete seat layout for a show.
- View only available seats.
- View only booked seats.
- Display physical seat information with availability status.
- Retrieve ticket price from the associated show.

### API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/shows/{showId}/seats | Get complete seat layout |
| GET | /api/shows/{showId}/seats/available | Get available seats |
| GET | /api/shows/{showId}/seats/booked | Get booked seats |

### Response Fields

- SeatId
- RowName
- SeatNumber
- SeatLabel
- SeatType
- Price
- Status

### Business Rules

- A ShowSeat belongs to one particular show.
- Every show has one ShowSeat record for each physical seat
  belonging to its screen.
- Newly generated ShowSeats have Available status.
- The ticket price is retrieved from the associated Show.
- Booking operations will update seat status in the Booking module.
- ## 7. Booking Management

### Features

- Create bookings for one or multiple seats.
- Validate show and seat availability.
- Prevent duplicate seat selection.
- Calculate booking amount on the backend.
- Generate unique booking references.
- Store booking and booking seat details.
- Update ShowSeats status to Booked.
- Retrieve booking details.
- Cancel confirmed bookings.
- Release seats after cancellation.
- Preserve cancelled booking history.
- Use database transactions for booking and cancellation.

### API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/bookings | Create a booking |
| GET | /api/bookings/{bookingId} | Get booking details |
| PUT | /api/bookings/{bookingId}/cancel | Cancel a booking |

### Booking Rules

1. A booking must have a valid ShowId.
2. At least one seat must be selected.
3. Duplicate SeatIds are not allowed.
4. All selected seats must belong to the selected show.
5. All selected seats must be Available.
6. The backend calculates the total amount.
7. Booking and BookingSeats are saved in one transaction.
8. ShowSeats are updated to Booked.
9. Only Confirmed bookings can be cancelled.
10. Cancellation releases seats and preserves booking history.

### Transaction Handling

Booking creation and cancellation use database transactions.

If an operation fails, the transaction is rolled back to prevent partial updates.

Version 1 does not include advanced concurrency protection for simultaneous booking requests.