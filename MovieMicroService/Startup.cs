using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MovieMicroService.Contracts;
using MovieMicroService.Models;
using MovieMicroService.Repositories;
using MovieMicroService.Services;
using System;

namespace MovieMicroService
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
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IMovieService, MovieService>();

            services.AddSingleton<IMongoClient, MongoClient>(f => new MongoClient(Configuration.GetConnectionString("mongoConnection")));

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<MovieService>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(provider);
                    config.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    config.ReceiveEndpoint("ReservationToMovieQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<MovieService>(provider);
                    });
                    config.ReceiveEndpoint("TicketToMovieQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<MovieService>(provider);
                    });
                }));
            });
            services.AddMassTransitHostedService();
            services.AddControllers().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
