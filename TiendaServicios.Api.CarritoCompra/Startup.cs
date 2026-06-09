using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TiendaServicios.Api.CarritoCompra.Aplicacion;
using TiendaServicios.Api.CarritoCompra.Persistencia;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;
using TiendaServicios.Api.CarritoCompra.RemoteService;

namespace TiendaServicios.Api.CarritoCompra
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("CorsRule", rule => {
                    rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
                });
            });

            services.AddScoped<ILibrosService, LibrosService>();
            services.AddScoped<IAutorService, AutorService>();
            services.AddScoped<IComicVineService, ComicVineService>();
            services.AddControllers();
            
            services.AddDbContext<CarritoContexto>(options =>
            {
                var connectionString = Configuration.GetConnectionString("ConexionDatabase");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Nuevo.Manejador).Assembly));
            
            services.AddHttpClient("Libros", config =>
            {
                var url = Configuration["Services:Libros"];
                if(!string.IsNullOrEmpty(url)) {
                   config.BaseAddress = new Uri(url);
                }
            });

            services.AddHttpClient("Autores", config =>
            {
                var url = Configuration["Services:Autores"];
                if(!string.IsNullOrEmpty(url)) {
                   config.BaseAddress = new Uri(url);
                }
            });

            services.AddHttpClient("ComicVine", config =>
            {
                var url = Configuration["ComicVine:BaseUrl"];
                if(!string.IsNullOrEmpty(url)) {
                   config.BaseAddress = new Uri(url);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsRule");

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