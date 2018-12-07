﻿using Autofac;
using System.Reflection;

namespace Alma.Exemplo.Aplicativo
{
    public class ConfigModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var ass = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(ass)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();
        }
    }
}
