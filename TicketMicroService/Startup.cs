using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TicketMicroService.Contracts;
using TicketMicroService.Models;
using TicketMicroService.Repositories;
using TicketMicroService.Services;

namespace TicketMicroService
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
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<ITicketService, TicketService>();

            services.AddDbContext<RepositoryDbContext>(options =>
                            options.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<TicketService>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(provider);
                    config.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    config.ReceiveEndpoint("MovieToTicketQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<TicketService>(provider);
                    });
                    config.ReceiveEndpoint("ReservationToTicketQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<TicketService>(provider);
                    });
                    config.ReceiveEndpoint("OrderToTicketQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<TicketService>(provider);
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
