using System;
using System.Linq;
using AutoMapper;

namespace Alma.Core.Mapper
{
    public static class AutoMapperConfig
    {
        public static void Setup()
        {
            AutoMapper.Mapper.Initialize(RunConfig);
        }

        private static void RunConfig(IMapperConfigurationExpression cfg)
        {
            //var nomeProjeto = "Vesta";
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic //caído.
                    && !x.FullName.Contains("Microsoft.")
                    && !x.FullName.Contains("System.")
                    ).ToArray();
            //.Where(a => a.FullName.StartsWith(nomeProjeto))
            //.ToArray();
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
                    IMapperHelper instancia = Activator.CreateInstance(map) as IMapperHelper;
                    instancia.Config(cfg);
                }
            }
        }
    }

}
