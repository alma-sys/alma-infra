using Alma.Common.Mapper;
using Alma.ExampleProject.Application.UserModule.Dto;
using Alma.ExampleProject.Domain.Entities;
using AutoMapper;

namespace Alma.ExampleProject.Application.UserModule.Maps
{
    public class UserToDtoMap : IMapperHelper
    {
        public void Config(IMapperConfigurationExpression cfg)
        {
            var map = cfg.CreateMap<User, UserDto>();

            map.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
