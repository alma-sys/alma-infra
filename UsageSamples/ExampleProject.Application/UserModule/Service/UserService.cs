using Alma.ExampleProject.Application.UserModule.Dto;
using Alma.ExampleProject.Domain.Entities;
using Alma.ExampleProject.Domain.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Application.UserModule.Service
{
    class UserService : IUserService
    {
        private IMapper mapper;
        private IUserRepository repository;

        public UserService(IMapper mapper, IUserRepository userRep)
        {
            this.mapper = mapper;
            this.repository = userRep;
        }

        public async Task<IList<UserDto>> List()
        {
            var lista = await repository.List();
            var dto = this.mapper.Map<List<UserDto>>(lista);
            return dto;
        }

        public async Task<UserDto> Save(UserDto dto)
        {
            var parentUser = (await repository.List()).FirstOrDefault(); // As an example, lets get anyone.


            var user = await repository.GetByDomainUserOrEmail(dto.Email);
            if (user == null)
                user = new User(dto.PersonUId, dto.Name, dto.FamilyName, dto.Address, dto.Email, dto.Telephone, dto.DomainUser);
            else
                user.Update(dto.Name, dto.FamilyName, dto.Address, dto.Telephone, dto.DomainUser);

            user.SetCreator(parentUser);

            user.UpdateLastAccess(DateTime.Now);

            user = await this.repository.Save(user);

            var instance = this.mapper.Map<UserDto>(user);
            return instance;
        }

    }
}
