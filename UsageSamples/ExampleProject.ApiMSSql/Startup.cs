using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Alma.ExampleProject.ApiMSSql
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables();



            this.Configuration = builder.Build();
        }
        public IContainer ApplicationContainer { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
            services.AddLogging();


            this.ApplicationContainer = DependencyResolverConfig.SetDependencyResolver(services, Configuration);

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
    IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SetCors(app);

            ConfigureAuth(app, this.ApplicationContainer);

            //SetRoutes(config);
            //AddFilters(config);

            //JobScheduler.Configure(app, config, container);
            app.UseMvc();

            app.Use((context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                return next();
            });
        }

        private static void SetCors(IApplicationBuilder app)
        {
            app.UseCors(a =>
            {
                a.AllowAnyHeader();
                a.AllowAnyMethod();
                a.AllowAnyOrigin();
            });
        }

        public static void ConfigureAuth(IApplicationBuilder app, IContainer container)
        {
            //var key = Alma.Common.Config.AppSettings["alma:jwtkey"];
            //var issuer = Alma.Common.Config.AppSettings["alma:jwtissuer"];
            //var audiences = Alma.Common.Config.AppSettings["alma:jwtaudiences"]?.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();

            //ConfigureBasicAuth(app, container);

            //ConfigureOAuthTokenServer(app, issuer, key);
            //ConfigureOAuthTokenConsumption(app, issuer, key, audiences);
        }

        //private static void ConfigureBasicAuth(IApplicationBuilder app, IContainer container)
        //{
        //    var provider = new BasicAuthProvider(config);
        //    var options = new BasicAuthenticationOptions("alma", provider.Validate);
        //    // options.AuthenticationMode = AuthenticationMode.Passive; // Isso evita que a janela de auth do browser apareça.

        //    app.UseBasicAuthentication(options);
        //}

        //public static void ConfigureOAuthTokenServer(IApplicationBuilder app, string issuer, string key)
        //{
        //    OAuthAuthorizationServerOptions oauthServerOptions = new OAuthAuthorizationServerOptions()
        //    {
        //        TokenEndpointPath = new PathString("/oauth2/token"),
        //        AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10),
        //        Provider = new OAuthProvider(),
        //        RefreshTokenProvider = new RefreshTokenProvider(),
        //        AccessTokenFormat = new CustomJwtFormat(issuer, key)
        //    };

        //    oauthServerOptions.AllowInsecureHttp = !Convert.ToBoolean(ConfigurationManager.ConnectionStrings["alma:https"]);
        //    // OAuth 2.0 Bearer Access Token Generation
        //    app.UseOAuthAuthorizationServer(oauthServerOptions);
        //}

        //private static void ConfigureOAuthTokenConsumption(IApplicationBuilder app, string issuer, string key, string[] audiences)
        //{
        //    byte[] audienceSecret = TextEncodings.Base64Url.Decode(key);

        //    // Api controllers with an [Authorize] attribute will be validated with JWT
        //    app.UseJwtBearerAuthentication(
        //        new JwtBearerAuthenticationOptions
        //        {
        //            AuthenticationMode = AuthenticationMode.Active,
        //            AllowedAudiences = audiences,
        //            IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
        //            {
        //                new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
        //            }
        //        });
        //}



        //private static void SetRoutes(HttpConfiguration config)
        //{
        //    //config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config));

        //    config.MapHttpAttributeRoutes();

        //    config.Routes.MapHttpRoute(
        //        name: "DefaultApiV1",
        //        routeTemplate: "api/{controller}/{id}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );
        //}

        //public static void AddFilters(HttpConfiguration config)
        //{
        //    config.Filters.Add(new SegurancaAttribute());
        //    config.Filters.Add(new ExceptionHandleAttribute());
        //    config.Filters.Add(new FiltroValidacoes());
        //}


    }
}
