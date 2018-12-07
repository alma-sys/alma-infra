using Autofac;

namespace Alma.ApiExtensions.Security
{
    public class SecurityModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new AdfsProvider())
                .AsImplementedInterfaces();

            builder.RegisterInstance(new JwtProvider())
                .AsImplementedInterfaces();
        }
    }
}
