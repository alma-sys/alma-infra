using Autofac;
using AutoMapper;

namespace Alma.Core.Mapper
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            AutoMapperConfig.Setup();


            builder.RegisterInstance(AutoMapper.Mapper.Instance).As<IMapper>();

        }

    }
}
