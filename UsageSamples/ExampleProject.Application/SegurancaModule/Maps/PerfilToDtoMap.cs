using Alma.Common.Mapper;
using Alma.Domain;
using Alma.Exemplo.Aplicativo.SegurancaModule.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alma.Exemplo.Aplicativo.SegurancaModule.Maps
{
    class PerfilToDtoMap : IMapperHelper
    {
        public void Config(IMapperConfigurationExpression cfg)
        {
            var map = cfg.CreateMap<Role, PerfilDto>();


        }
    }
}
