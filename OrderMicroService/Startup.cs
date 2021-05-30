using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderMicroService.Contracts;
using OrderMicroService.Models;
using OrderMicroService.Repositories;
using OrderMicroService.Services;
using System;

namespace OrderMicroService
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
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddDbContext<RepositoryDbContext>(options =>
                            options.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderService>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(provider);
                    config.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    config.ReceiveEndpoint("TicketToOrderQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<OrderService>(provider);
                    });
                    config.ReceiveEndpoint("ReservationToOrderQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(100, 100));
                        ep.ConfigureConsumer<OrderService>(provider);
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
