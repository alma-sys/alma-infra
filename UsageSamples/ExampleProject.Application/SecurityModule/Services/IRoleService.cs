using Alma.ExampleProject.Application.SecurityModule.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Application.SecurityModule.Services
{
    public interface IRoleService
    {
        Task Save(RoleDto user);
        Task<IList<RoleDto>> List();

    }
}