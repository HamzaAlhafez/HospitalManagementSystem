# Hospital Management System API

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A secure RESTful API for hospital management built with ASP.NET Core 8, featuring JWT authentication, role-based authorization, and comprehensive appointment management.

## Key Improvements & Patterns
- Repository Pattern: Implemented for data access abstraction
- Serilog Logging: Structured logging with file and console sinks
- Rate Limiting: Protection against brute-force attacks (5 requests/10s per client)
- Postman Integration: Complete API testing collection included

## Features

### Core Functionality
- ✅ JWT Authentication with role-based authorization
- ✅ Refresh Token mechanism
- ✅ Email notifications with attachments
- ✅ Statistics dashboard for administrators
- ✅ Pagination

### Role-Based Access
| Role       | Permissions |
|------------|-------------|
| Admin      | Manage all entities, view statistics |
| Doctor     | Manage appointments, confirm/complete sessions |
| Patient    | Book appointments, rate doctors |

### Technical Highlights
`mermaid
graph LR
A[Repository Pattern] --> B[Service Layer]
B --> C[Controllers]
D[Serilog] --> E[Structured Logging]
F[Rate Limiting] --> G[API Protection]
H[Postman] --> I[Testing Collection]
## Postman Collection
[![Run in Postman](https://run.pstmn.io/button.svg)](https://hamza-456417.postman.co/workspace/hamza's-Workspace~c56550d1-c028-4e83-8a37-2a48c5b4262d/collection/44009679-37e27876-3a39-42ff-8ebd-273cfdd4d4cf?action=share&source=collection_link&creator=44009679)

Complete collection includes:
- Authentication workflow (Login/Refresh/Logout)
- CRUD operations for all entities
- Role-specific endpoints
- Pre-configured environment variables
- Example requests for all endpoints

**Collection Structure:**
1. **Admins** - Manage admin accounts
2. **Auth** - Authentication endpoints
3. **Appointments** (Admin/Doctor/Patient roles)
4. **Doctors** - Doctor management
5. **Patients** - Patient management
6. **Reviews** - Rating system
7. **Stats** - Statistical data
8. **Users** - Password management
9. **Mailing** - Email services with attachments

## Technologies
- ASP.NET Core 8
- Entity Framework Core
- JWT Authentication
- Serilog
- AspNetCoreRateLimit
- MailKit
- Swagger UI
- postman


## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- SMTP credentials (for email)

### Installation
1. Clone repository: git clone https://github.com/HamzaAlhafez/HospitalManagementSystem.git
2.  update Configure appsettings.json
3.  Apply migrations: bash
   dotnet ef database update
4. Run application:

## License
This project is licensed under the MIT License - see [LICENSE](LICENSE) for details.

   

  
   



  
      
   

