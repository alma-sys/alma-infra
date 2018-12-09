using Alma.Common.Mapper;
using Alma.Domain;
using Alma.ExampleProject.Application.SecurityModule.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alma.ExampleProject.Application.SecurityModule.Maps
{
    class RoleToDtoMap : IMapperHelper
    {
        public void Config(IMapperConfigurationExpression cfg)
        {
            var map = cfg.CreateMap<Role, RoleDto>();


        }
    }
}
