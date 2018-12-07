using AutoMapper;
using System;
using System.Linq;

namespace Alma.Core.Mapper
{
    static class AutoMapperConfig
    {
        private static bool alreadyStarted;

        public static void Setup()
        {
            if (alreadyStarted)
                return;
            AutoMapper.Mapper.Initialize(RunConfig);
            alreadyStarted = true;
        }

        private static void RunConfig(IMapperConfigurationExpression cfg)
        {
            //Trace.WriteLine("[automapper] Iniciando configuração do automapper");
            //var assemblies = Config.GetReferencingAssemblies()
            //    .Where(x => !x.IsDynamic //caído.
            //        && !x.FullName.Contains("Microsoft.")
            //        && !x.FullName.Contains("System.")
            //        ).ToArray();

            var assemblies = Config.AssembliesMapeadas.SelectMany(x => x.Value).ToArray();


            //assemblies.AsQueryable().AsParallel().ForAll(x =>
            //{
            //    Trace.WriteLine($"[automapper] Assembly {x.FullName} selecionado");
            //});


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
                    //Trace.WriteLine($"[automapper] Map {map.Name } sendo carregado");
                    instancia.Config(cfg);
                }
            }
        }
    }

}
