# Hospital Management System API

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A secure RESTful API for hospital management built with ASP.NET Core 8, featuring JWT authentication, role-based authorization, and comprehensive appointment management.

## Features

### Core Functionality
- ✅ JWT Authentication with role-based authorization
- ✅ Refresh Token mechanism
- ✅ Email notifications for appointments
- ✅ Statistics dashboard for administrators

### Role-Based Access
| Role       | Permissions |
|------------|-------------|
| Admin      | Manage doctors/patients, create appointments, view statistics |
| Doctor     | Accept/reject appointments, view patient history |
| Patient    | Request appointments, rate doctors |

### Key Endpoints
- Appointments Management
  - Book/Cancel appointments (Patient)
  - Accept/Reject appointments (Doctor)
  - View all appointments (Admin)
- Rating System
  - Patients can rate doctors (1-5 stars)
  - View average ratings per doctor
- Email Notifications
  - Appointment confirmations
  - Status updates

## Technologies
- ASP.NET Core 8
- Entity Framework Core
- JWT Authentication
- Dtos
- Swagger UI
- MailKit (Email services)
  



## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- SMTP server credentials (for email)
