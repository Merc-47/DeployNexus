using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeployNexus.Application.Common;
using DeployNexus.Application.Organizations.DTOs;

namespace DeployNexus.Application.Organizations.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request);

    Task<OrganizationDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<OrganizationDto>> GetAllAsync();

    Task<OrganizationDto?> UpdateAsync(Guid id, UpdateOrganizationRequest request);

    Task<Result> DeactivateAsync(Guid id);
}
