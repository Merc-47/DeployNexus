using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeployNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureDeployNexusSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        UPDATE Roles
        SET RoleType = 1
        WHERE Id = '196D095B-9BF1-4BDE-908B-6FA278F40A10'
          AND Name = 'Super Admin'
          AND OrganizationId = '5A18E8D4-E0CB-4314-8B32-9268C0377FEA';
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        UPDATE Roles
        SET RoleType = 0
        WHERE Id = '196D095B-9BF1-4BDE-908B-6FA278F40A10'
          AND Name = 'Super Admin'
          AND OrganizationId = '5A18E8D4-E0CB-4314-8B32-9268C0377FEA';
    """);
        }
    }
}
