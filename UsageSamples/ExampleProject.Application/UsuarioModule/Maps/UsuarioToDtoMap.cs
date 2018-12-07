using Alma.Common.Mapper;
using Alma.Exemplo.Aplicativo.UsuarioModule.Dto;
using Alma.Exemplo.Dominio.Entidades;
using AutoMapper;

namespace Alma.Exemplo.Aplicativo.UsuarioModule.Maps
{
    public class UsuarioToDtoMap : IMapperHelper
    {
        public void Config(IMapperConfigurationExpression cfg)
        {
            var map = cfg.CreateMap<Usuario, UsuarioDto>();

            map.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
