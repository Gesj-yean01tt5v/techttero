﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.Injector;
using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace AspectCore.Extensions.Autofac.WebSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();

            services.AddTransient<IValuesService, ValuesService>();

            ContainerBuilder builder = new ContainerBuilder();

            builder.Populate(services);

            builder.RegisterDynamicProxy(config =>
            {
                config.Interceptors.AddTyped<MethodExecuteLoggerInterceptor>(Predicates.ForNameSpace("AspectCore.Extensions.Autofac.*"));
            });

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
