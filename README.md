# StudyTeknik - Backend API

A comprehensive study technique management system built with ASP.NET Core 8.0, implementing Clean Architecture principles with CQRS pattern using MediatR.

## Table of Contents

- [Project Description](#project-description)
- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Database Setup](#database-setup)
  - [Backend Setup](#backend-setup)
- [API Endpoints](#api-endpoints)
- [Authentication & Authorization](#authentication--authorization)
- [Known Issues](#known-issues)
- [Contributing](#contributing)

## Project Description

StudyTeknik is a backend API designed to help students and teachers manage their study activities, including:

- **User Management**: Support for students, teachers, and administrators with role-based access control
- **Deck & Flashcard System**: Create and manage study decks with flashcards for effective learning
- **Diary Entries**: Track daily study activities and reflections
- **Student Profiles**: Personalized profiles with study preferences and schedules
- **AI Integration**: AI-powered text rewriting capabilities for study materials
- **Class Management**: Teachers can manage classes and enrollments

The system follows modern software engineering practices with:
- Clean Architecture for maintainability and testability
- CQRS pattern for separation of read/write operations
- Repository pattern for data access abstraction
- JWT-based authentication with Logto integration
- Comprehensive validation using FluentValidation

## Architecture Overview

The project follows Clean Architecture principles with clear separation of concerns:

```
StudyTeknik/
├── src/
│   ├── StudyTeknik/              # Presentation Layer (API Controllers)
│   │   ├── Controllers/          # REST API endpoints
│   │   ├── Middleware/           # Custom middleware (auth, logging)
│   │   └── Program.cs           # Application entry point
│   │
│   ├── Application/              # Application Layer (Business Logic)
│   │   ├── Commands/            # Write operations (CQRS)
│   │   ├── Queries/             # Read operations (CQRS)
│   │   ├── Dtos/                # Data Transfer Objects
│   │   ├── Abstractions/        # Interfaces
│   │   └── Common/              # Shared utilities, behaviors, results
│   │
│   ├── Domain/                   # Domain Layer (Core Business Rules)
│   │   ├── Entities/            # Domain entities
│   │   ├── Abstractions/        # Domain interfaces
│   │   └── Models/              # Value objects and domain models
│   │
│   └── Infrastructure/           # Infrastructure Layer (External Concerns)
│       ├── Persistence/         # Database context, repositories
│       ├── Migrations/          # EF Core migrations
│       ├── Service/             # External service integrations
│       └── DependencyInjection/ # DI configuration
│
└── test/
    └── StudyTeknik.Test/        # Unit and integration tests
```

### Key Architectural Patterns

- **CQRS (Command Query Responsibility Segregation)**: Separates read and write operations using MediatR
- **Repository Pattern**: Abstracts data access logic
- **Dependency Injection**: Uses built-in .NET DI container
- **Result Pattern**: Type-safe error handling without exceptions
- **Validation Pipeline**: Automatic validation using FluentValidation behaviors
- **Clean Architecture**: Dependencies point inward (Domain ← Application ← Infrastructure/Presentation)

## Technology Stack

### Backend
- **.NET 8.0** - Latest LTS version of .NET
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 8.0** - ORM for database access
- **SQL Server** - Primary database (LocalDB/SQL Server Express)
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Newtonsoft.Json** - JSON serialization with PATCH support

### Authentication & Security
- **JWT Bearer Authentication** - Token-based authentication
- **Logto** - Identity provider integration
- **Role-based Authorization** - Admin, Teacher, Student roles

### Development Tools
- **Swagger/OpenAPI** - API documentation (Development only)
- **Entity Framework Migrations** - Database version control

## Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server** (Express, Developer, or LocalDB)
  - [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
  - Or SQL Server LocalDB (included with Visual Studio)
- **Git** for version control
- **Visual Studio 2022** or **VS Code** (optional but recommended)

## Getting Started

### Database Setup

1. **Install SQL Server**

   If you haven't already, install SQL Server Express or LocalDB.

2. **Update Connection String**

   Edit the connection string in `src/StudyTeknik/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=StudyTeknikDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
     }
   }
   ```

   Replace `YOUR_SERVER_NAME` with your SQL Server instance name:
   - LocalDB: `(localdb)\\MSSQLLocalDB`
   - SQL Server Express: `YOUR_COMPUTER_NAME\\SQLEXPRESS`
   - SQL Server: `localhost` or your server name

3. **Apply Database Migrations**

   ```bash
   cd src/StudyTeknik
   dotnet ef database update
   ```

   This will create the `StudyTeknikDB` database and apply all migrations.

### Backend Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/MDaskalou/StudyTeknikBE.git
   cd StudyTeknikBE
   ```

2. **Restore Dependencies**

   ```bash
   dotnet restore
   ```

3. **Configure JWT Authentication (Optional for Development)**

   The application uses Logto for authentication. Update JWT settings in `appsettings.Development.json`:

   ```json
   {
     "Jwt": {
       "Authority": "https://9ixsif.logto.app/oidc",
       "Audience": "api://studyteknik"
     }
   }
   ```

4. **Build the Solution**

   ```bash
   dotnet build
   ```

5. **Run the Application**

   ```bash
   cd src/StudyTeknik
   dotnet run
   ```

   The API will start on:
   - HTTPS: `https://localhost:44317`
   - HTTP: `http://localhost:5196`

6. **Access Swagger UI** (Development Only)

   Open your browser and navigate to:
   ```
   https://localhost:44317/swagger
   ```

   This provides an interactive API documentation and testing interface.

### Running Tests

To run the test suite:

```bash
dotnet test
```

## API Endpoints

All endpoints require authentication unless otherwise specified. Include the JWT token in the `Authorization` header:

```
Authorization: Bearer <your_jwt_token>
```

### Authentication

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| *Planned* | `/api/auth/me` | Get current user information | Required |

> **Note**: Authentication endpoints are currently handled by Logto (external IdP).

### Students Management

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/students/GetAllStudents` | Get all students | Admin only |
| GET | `/api/students/GetStudentById/{id}` | Get student by ID | Admin only |
| POST | `/api/students/CreateStudent` | Create new student | Admin only |
| PUT | `/api/students/UpdateStudent/{id}` | Update student | Admin only |
| PATCH | `/api/students/UpdateStudentDetails/{id}` | Partially update student | Admin only |
| DELETE | `/api/students/DeleteStudent/{id}` | Delete student | Admin only |
| GET | `/api/students/student/general` | Get current student's general info | Authenticated |

### Teachers Management

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/teachers/GetAllTeachers` | Get all teachers | Admin only |
| GET | `/api/teachers/{id}` | Get teacher by ID | Admin only |
| POST | `/api/teachers/CreateTeacher` | Create new teacher | Admin only |
| PUT | `/api/teachers/UpdateTeacher/{id}` | Update teacher | Admin only |
| PATCH | `/api/teachers/UpdateTeacher/{id}` | Partially update teacher | Admin only |
| DELETE | `/api/teachers/DeleteTeacher/{id}` | Delete teacher | Admin only |

### Decks (Study Card Decks)

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/decks/GetAllDecks` | Get all decks for current user | Authenticated |
| GET | `/api/decks/GetDeckById/{id}` | Get specific deck | Authenticated |
| POST | `/api/decks/CreateDeck` | Create new deck | Authenticated |
| PUT | `/api/decks/UpdateDeck/{id}` | Update deck | Authenticated |
| PATCH | `/api/decks/UpdateDeckDetails/{id}` | Partially update deck | Authenticated |
| DELETE | `/api/decks/DeleteDeck/{id}` | Delete deck | Authenticated |

### Flashcards

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/decks/{deckId}/flashcards` | Get all flashcards in a deck | Authenticated |
| POST | `/api/decks/{deckId}/flashcards` | Add flashcard to deck | Authenticated |

**Request Body Example** (POST):
```json
{
  "frontText": "Question or term",
  "backText": "Answer or definition"
}
```

### Diary Entries

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/diary/GetAllDiariesForStudent` | Get all diary entries for current user | Authenticated |
| GET | `/api/diary/GetGiaryById/{id}` | Get specific diary entry | Authenticated |
| POST | `/api/diary/CreateDiary` | Create new diary entry | Authenticated |
| PUT | `/api/diary/UpdateDiary/{id}` | Update diary entry | Authenticated |
| PATCH | `/api/diary/UpdateDiaryDetails/{id}` | Partially update diary entry | Authenticated |
| DELETE | `/api/diary/DeleteDiary/{id}` | Delete diary entry | Authenticated |

**Request Body Example** (POST):
```json
{
  "entryDate": "2024-01-15T10:30:00Z",
  "text": "Today I studied calculus for 2 hours..."
}
```

### Student Profiles

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/student-profiles/GetAllStudentProfiles` | Get all student profiles | Authenticated |
| POST | `/api/student-profiles` | Create student profile | Authenticated |

**Request Body Example** (POST):
```json
{
  "planningHorizonWeeks": 4,
  "wakeUpTime": "07:00:00",
  "bedTime": "23:00:00"
}
```

### AI Features

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/ai/Rewrite` | Rewrite text using AI | Authenticated |

**Request Body Example**:
```json
{
  "text": "Your text to be rewritten for better clarity"
}
```

### Classes (Teacher Feature)

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| *Planned* | `/api/classes/mine` | Get classes for current teacher | Teacher only |

> **Note**: Class endpoints are currently under development.

## Authentication & Authorization

### JWT Authentication

The API uses JWT (JSON Web Tokens) for authentication via an external identity provider (Logto).

**Getting a Token:**
1. Authenticate with Logto at: `https://9ixsif.logto.app/oidc`
2. Obtain a JWT token with audience `api://studyteknik`
3. Include the token in the `Authorization` header for all requests

**Example Request:**
```bash
curl -X GET "https://localhost:44317/api/decks/GetAllDecks" \
  -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Authorization Roles

The system supports three user roles:

- **Admin**: Full access to user management (students, teachers)
- **Teacher**: Access to class management and student oversight
- **Student**: Access to personal study materials (decks, diary, profile)

### Swagger Authorization

When using Swagger UI in development:

1. Click the **Authorize** button (top right)
2. Enter: `Bearer <your_token>`
3. Click **Authorize**

All subsequent requests will include the token automatically.

## Known Issues

### 1. Typo in Endpoint
- **Issue**: Diary endpoint has typo: `/api/diary/GetGiaryById/{id}` (should be "GetDiaryById")
- **Impact**: Low - endpoint works but has incorrect spelling
- **Status**: Documented

### 2. Incomplete Controllers
- **Issue**: `AuthController`, `UsersController`, and `WeeklySummariesController` are empty placeholders
- **Impact**: Medium - planned features not yet implemented
- **Status**: Marked as TODO

### 3. Class Management Endpoints
- **Issue**: `/api/classes/mine` endpoint is commented out
- **Impact**: Medium - teacher functionality limited
- **Status**: Under development

### 4. Development Authentication
- **Issue**: Authentication middleware has commented-out code for development mode
- **Impact**: Low - authentication is currently always enforced
- **Status**: Design decision, may need flexibility for local development

### 5. Database Seeding
- **Issue**: Database seeding only runs in Development environment
- **Impact**: Low - expected behavior, but may need test data for staging
- **Status**: Working as intended

### 6. Swedish Comments in Code
- **Issue**: Some comments and console messages are in Swedish
- **Impact**: Low - doesn't affect functionality
- **Status**: Documentation issue

### 7. No Frontend in This Repository
- **Issue**: This is a backend-only repository
- **Impact**: N/A - requires separate frontend application
- **Status**: By design - frontend should be in separate repository

## Project Structure Details

### Domain Entities

The system manages the following core entities:

- **UserEntity**: Base user with role (Student/Teacher/Admin)
- **StudentProfileEntity**: Student-specific preferences and settings
- **DeckEntity**: Collection of flashcards
- **FlashCardEntity**: Individual study cards
- **DiaryEntity**: Student diary entries
- **ClassEntity**: Teacher-managed classes
- **EnrollmentEntity**: Student-class relationships
- **WeeklySummaryEntity**: Weekly study summaries (planned)
- **AuditLogEntity**: System audit trail
- **StudyGoalsEntity**: Student learning objectives (planned)
- **StudyPlanTasksEntity**: Planned study tasks (planned)
- **StudySessionsEntity**: Study session tracking (planned)

### Validation & Error Handling

- **FluentValidation**: Automatic validation in MediatR pipeline
- **Result Pattern**: Type-safe errors without exceptions
- **Error Types**: Validation, NotFound, Conflict, Forbidden
- **HTTP Status Mapping**: Automatic mapping of errors to appropriate HTTP responses

### Database Migrations

To create a new migration:

```bash
cd src/StudyTeknik
dotnet ef migrations add MigrationName
```

To apply migrations:

```bash
dotnet ef database update
```

To rollback:

```bash
dotnet ef database update PreviousMigrationName
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow C# naming conventions
- Use async/await for all I/O operations
- Write unit tests for business logic
- Keep controllers thin - business logic belongs in handlers
- Use nullable reference types

## License

*License information not specified - please add appropriate license file*

## Contact

For questions or support, please open an issue in the GitHub repository.

---

**Last Updated**: December 2024  
**Version**: 1.0  
**.NET Version**: 8.0  
**Database**: SQL Server
