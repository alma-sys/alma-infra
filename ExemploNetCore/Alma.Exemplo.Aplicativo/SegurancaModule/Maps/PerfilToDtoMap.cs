using Alma.Core.Mapper;
using Alma.Dominio;
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
            var map = cfg.CreateMap<Perfil, PerfilDto>();


        }
    }
}
