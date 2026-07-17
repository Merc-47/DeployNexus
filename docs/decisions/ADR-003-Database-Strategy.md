# ADR-003: Database Strategy


## Status

Accepted


## Date

2026-07-17


## Decision

Deploy Nexus will use Microsoft SQL Server as the primary relational database system.

The database design will follow a modular approach where each application module owns and manages its related data structures.


## Context

Deploy Nexus is designed as a long-term enterprise platform that will support multiple business modules.

The database strategy must support:

- Data consistency
- Security
- Scalability
- Maintainability
- Future module expansion


## Database Approach

The initial version will use a single database with logical separation between modules.

Example:

DeployNexus Database

- Identity tables
- User management tables
- Organization tables
- Audit tables
- Module-specific tables


## Data Access Strategy

Entity Framework Core will be used as the Object Relational Mapper.

Responsibilities:

- Entity management
- Database migrations
- Query handling
- Transaction management


## Database Migration Strategy

All database changes must be version controlled.

Changes will be managed through:

- Entity Framework migrations
- Migration history
- Reviewed database changes


Example:

Migration:

001_CreateUsersTable

002_CreateRolesTable

003_AddPermissions


## Naming Convention

Database objects will follow consistent naming.

Tables:

Users

Roles

Permissions


Columns:

UserId

CreatedDate

UpdatedDate

IsActive


## Backup Strategy

Production databases should support:

- Regular backups
- Recovery testing
- Backup retention policies


## Security Considerations

Database security requirements:

- Restricted database access
- Separate user permissions
- Encrypted sensitive information
- Secure connection strings


## Alternatives Considered


### Database Per Module

Rejected initially because:

- Higher operational complexity
- More difficult transaction management
- Not required for initial scale


### NoSQL Database

Rejected because:

- Core platform data requires relational relationships
- Strong consistency is required


## Future Considerations

The architecture should allow:

- Database optimization
- Read replicas
- Caching layers
- Module-specific databases if required