using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployNexus.Domain.Interfaces;

public interface IEntity
{
    Guid Id { get; set; }
}