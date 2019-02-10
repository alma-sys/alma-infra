using Autofac;
using AutoMapper;
using System;
using System.Diagnostics;
using System.Linq;

namespace Alma.Common.Mapper
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            Setup();


            builder.RegisterInstance(AutoMapper.Mapper.Instance).As<IMapper>();

        }

        private static bool alreadyStarted;

        private static void Setup()
        {
            if (alreadyStarted)
                return;
            AutoMapper.Mapper.Initialize(RunConfig);
            alreadyStarted = true;
        }

        private static void RunConfig(IMapperConfigurationExpression cfg)
        {

            var assemblies = Config.MappedAssemblies.SelectMany(x => x.Value).ToArray();

            var target = typeof(IMapperHelper);
            foreach (var item in assemblies)
            {
                var maps = item
                    .GetTypes()
                    .Where(t =>
                        t != target &&
                        target.IsAssignableFrom(t)
                        ).ToList();

                foreach (var map in maps)
                {
                    IMapperHelper obj = Activator.CreateInstance(map) as IMapperHelper;
                    Trace.WriteLine($"[automapper] Loading map '{ map.Name }'...");
                    obj.Config(cfg);
                }
            }
        }
    }
}
