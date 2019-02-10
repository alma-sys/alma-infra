using Alma.Domain;
using Alma.ExampleProject.Application.SecurityModule.Dto;
using Alma.ExampleProject.Domain.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.Application.SecurityModule.Services
{
    class RoleService : Alma.Application.ServiceBase, IRoleService
    {
        private IMapper mapper;
        private IRoleRepository repository;
        private IAuthorizationRepository authorizationRep;

        public RoleService(IMapper mapper, IRoleRepository repository, IAuthorizationRepository authorizationRep)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.authorizationRep = authorizationRep;

            CheckAuthorization();
        }

        private const string ROOT = nameof(ROOT);
        private const string USER_ACCESS = nameof(USER_ACCESS);
        private const string ROLE_ACCESS = nameof(ROLE_ACCESS);

        /// <summary>
        /// Check if all authorization tags are created.
        /// </summary>
        private void CheckAuthorization()
        {


            this.authorizationRep.UpdateAvailableAuthorizations(new string[] {
                ROOT,
                ROLE_ACCESS,
                USER_ACCESS
            });

        }

        public async Task<IList<RoleDto>> List()
        {
            var lista = await repository.ListAsync();
            var dto = this.mapper.Map<List<RoleDto>>(lista);
            return dto;
        }

        public async Task Save(RoleDto dto)
        {
            var role = await repository.GetByName(dto.Name);
            if (role == null)
                role = new Role(dto.Name, dto.Description, dto.Enabled);
            else
            {
                role.ChangeDescription(dto.Description);
                role.ChangeName(dto.Name);


                if (dto.Enabled)
                    role.Enable();
                else
                    role.Disable();
            }


            //como este é um exemplo, associar aleatoriamente permissões
            var authorizations = await authorizationRep.List();
            var rnd = new Random();
            foreach (var p in authorizations)
            {
                if (rnd.Next(0, 2) == 1)
                {
                    role.AddAccess(p);
                }
            }

            await this.repository.SaveAsync(role);

            /* return */
            this.mapper.Map<RoleDto>(role);
        }
    }
}
