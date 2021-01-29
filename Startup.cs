using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Webapi.Data;
using AutoMapper;
using Webapi.Mapper;
using Webapi.Commands;
using Webapi.Queries;
using System.Collections.Generic;
using Webapi.Models;
using Webapi.Utils;
using Webapi.Decorators;

namespace Webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<Messages>();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            , ServiceLifetime.Singleton // not good practice -> following tutorial as it is
            );

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            var config = new Config(3);
            services.AddSingleton(config);

            // >> FIRST IMPLEMENTATION <<
            // services.AddTransient<ICommandHandler<EditStatusCommand>, EditStatusCommandHandler>();

            // >> SECOND IMPLEMENTATION <<
            // services.AddTransient<ICommandHandler<EditStatusCommand>>(provier =>
            //     new DatabaseRetryDecorator<EditStatusCommand>(
            //         new EditStatusCommandHandler(provier.GetService<ApplicationDbContext>(), provier.GetService<IMapper>()), provier.GetService<Config>()
            // ));

            // >> THIRD IMPLEMENTATION <<
            // services.AddTransient<ICommandHandler<RegisterCommand>>(provier =>
            //     new AuditLogginDecorator<RegisterCommand>(
            //        new DatabaseRetryDecorator<RegisterCommand>(
            //           new RegisterCommandHandler(provier.GetService<ApplicationDbContext>(), provier.GetService<IMapper>()), provier.GetService<Config>()
            //  )));

            // >> FORTH IMPLEMENTATION <<
            services.AddHandlers();

            // >> NOTE <<
            // -> this part is for ISessionFactory which I don't have cause I use 
            // var connectionString = new ConnectionString(Configuration.GetConnectionString("DefaultConnection"));
            // services.AddSingleton(connectionString); 

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Webapi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webapi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
