## Theatre Management API

The Theatre Management module provides operations to create, retrieve, update, and delete theatre information.

### Theatre API Endpoints

| Operation | HTTP Method | Endpoint |
|---|---|---|
| Create Theatre | POST | `/api/theatres` |
| Get All Theatres | GET | `/api/theatres` |
| Get Theatre By ID | GET | `/api/theatres/{id}` |
| Update Theatre | PUT | `/api/theatres/{id}` |
| Delete Theatre | DELETE | `/api/theatres/{id}` |

### Theatre Database Fields

| Column | Data Type | Constraint |
|---|---|---|
| TheatreId | int | Primary Key, Identity |
| Name | nvarchar(150) | Required |
| Location | nvarchar(200) | Required |

### Theatre API Testing

The Theatre Management endpoints were tested using Postman.

- Create Theatre – 201 Created
- Get All Theatres – 200 OK
- Get Theatre By ID – 200 OK
- Update Theatre – 200 OK
- Delete Theatre – 204 No Content
- Verify Theatre Deletion – 404 Not Found

The exported Postman collection is maintained in the `Postman` folder.