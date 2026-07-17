# ADR-006: Development Environment


## Status

Accepted


## Date

2026-07-17


## Decision

Deploy Nexus development environment will use:

Backend:
.NET 8 LTS with ASP.NET Core Web API

Frontend:
React with TypeScript using Vite

Database:
Microsoft SQL Server

ORM:
Entity Framework Core

Version Control:
Git with GitHub


## Reasoning

The selected technologies provide:

- Enterprise adoption
- Long-term support
- Strong developer ecosystem
- Maintainable architecture
- Scalability


## Development Tools

Recommended tools:

Backend:
Visual Studio 2022

Frontend:
Visual Studio Code

Database:
SQL Server Management Studio


## Future Considerations

The environment should support:

- Docker development containers
- CI/CD pipelines
- Cloud deployment