# ADR-002: Technology Stack


## Status

Accepted


## Date

2026-07-17


## Decision

Deploy Nexus will use the following technology stack:


## Backend

Technology:

ASP.NET Core Web API


Reason:

- Enterprise-grade framework
- High performance
- Strong ecosystem
- Excellent support for REST APIs
- Suitable for scalable applications


## Frontend

Technology:

React with TypeScript


Reason:

- Component-based architecture
- Large ecosystem
- Strong industry adoption
- Suitable for complex user interfaces


## Database

Technology:

Microsoft SQL Server


Reason:

- Enterprise database platform
- Strong relational data support
- Good integration with .NET ecosystem


## ORM

Technology:

Entity Framework Core


Reason:

- Reduces database access complexity
- Supports migrations
- Integrates well with ASP.NET Core


## Authentication

Technology:

JWT-based authentication


Reason:

- Stateless authentication
- Suitable for API-based applications
- Supports modern frontend architectures


## Logging

Technology:

Structured application logging


Reason:

- Easier troubleshooting
- Better production monitoring
- Supports audit requirements


## Containerization

Technology:

Docker


Reason:

- Consistent environments
- Easier deployment
- Reduces configuration issues


## Version Control

Technology:

Git


Reason:

- Industry standard
- Supports collaboration
- Enables version history


## Alternatives Considered


### Backend Alternatives

Spring Boot:

Rejected for this project because Deploy Nexus will focus on the .NET ecosystem.


Node.js:

Rejected because ASP.NET Core provides stronger alignment with enterprise C# development.


### Frontend Alternatives

Angular:

Not selected because React provides more flexibility for this project.


## Future Considerations

The architecture should allow future integration with:

- Cloud hosting platforms
- CI/CD pipelines
- Message queues
- AI services
- Additional frontend applications