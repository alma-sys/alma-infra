using Alma.ExampleProject.Application.UserModule.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Application.UserModule.Service
{
    public interface IUserService
    {
        Task<UserDto> Save(UserDto user);
        Task<IList<UserDto>> List();
    }
}