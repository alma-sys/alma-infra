using System;
using System.Linq;
using AutoMapper;
using System.Diagnostics;

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
            Trace.WriteLine("[automapper] Iniciando configuração do automapper");
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic //caído.
                    && !x.FullName.Contains("Microsoft.")
                    && !x.FullName.Contains("System.")
                    ).ToArray();

            assemblies.AsParallel().ForAll(x =>
            {
                Trace.WriteLine($"[automapper] Assembly {x.FullName} selecionado");
            });


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
                    Trace.WriteLine($"[automapper] Map {map.Name } sendo carregado");
                    instancia.Config(cfg);
                }
            }
        }
    }

}
