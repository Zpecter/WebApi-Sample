﻿using System;
using System.IO;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using Api;
using Api.Infrastructure.Authorization;
using Autofac;
using Autofac.Integration.WebApi;
using Infrastructure;
using Microsoft.Owin.Hosting.Tracing;
using Microsoft.Owin.Security.Authorization.Infrastructure;
using Owin;
using Swashbuckle.Application;

namespace ConsoleHost
{
    public class Startup
    {
        private IContainer _container;

        public void Configuration(IAppBuilder app)
        {
            _container = ConfigureAndBuildContainer();

            app.UseAutofacMiddleware(_container);
            app.DisposeScopeOnAppDisposing(_container);

            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(_container)
            };

            ApiConfiguration(app);

            var apiExplorer = config.AddVersionedApiExplorer();

            config.EnableSwagger(
                "{apiVersion}/swagger",
                swagger =>
                {
                    // build a swagger document and endpoint for each discovered API version
                    swagger.MultipleApiVersions(
                        (apiDescription, version) =>
                            apiDescription.GetGroupName() == version,
                            info =>
                            {
                                foreach (var group in apiExplorer.ApiDescriptions)
                                {
                                    var apiVersion = group.ApiVersion;
                                    var description = "Cinematic api.";

                                    if (group.IsDeprecated)
                                    {
                                        description += " This API version has been deprecated.";
                                    }

                                    info.Version(group.Name, $"Cinematic API {group.ApiVersion}")
                                        .Contact(c => c.Name("Hugo Biarge").Email("hbiarge@plainconcepts.com"))
                                        .Description(description)
                                        .License(l => l.Name("MIT").Url("https://opensource.org/licenses/MIT"))
                                        .TermsOfService("Shareware");
                                }
                            });


                    // add a custom operation filter which documents the implicit API version parameter
                    //swagger.OperationFilter<SwaggerDefaultValues>();
                    // swagger.OperationFilter<ImplicitApiVersionParameter>();

                    // integrate xml comments
                    // swagger.IncludeXmlComments(XmlCommentsFilePath);
                })
                .EnableSwaggerUi();

            app.UseWelcomePage();
        }

        private static IContainer ConfigureAndBuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ApiAutofacModule
            {
                SampleConfiguration = true
            });
            builder.RegisterModule<InfrastructureAutofacModule>();

            return builder.Build();
        }

        private void ApiConfiguration(IAppBuilder api)
        {
            // Create HttpConfiguration
            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(_container)
            };

            // Add policy based authorization
            api.UseAuthorization(Policies.Configure);

            // Configure common options
            Api.ApiConfiguration.Configure(config);

            // Configure middlewares pipeline

            // ### Temporal authentication middleware
            api.Use(async (context, next) =>
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "Administrator"), 
                    new Claim(ClaimTypes.Role, "Vendor"), 
                    new Claim(ClaimTypes.Name, "Hugo"),
                },
                "Custom");
                context.Authentication.User = new ClaimsPrincipal(identity);
                await next();
            });
            // ###

            api.UseAutofacWebApi(config);
            api.UseWebApi(config);
        }
    }
}
