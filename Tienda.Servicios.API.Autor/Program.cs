using TiendaServicios.Api.Autor;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// 1. Instanciamos tu clase Startup pasándole la configuración del sistema
var startup = new Startup(builder.Configuration);

// 2. Ejecutamos tu método ConfigureServices para registrar MediatR, Postgres, etc.
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// 3. Ejecutamos tu método Configure para establecer las rutas y el entorno
startup.Configure(app, app.Environment);

app.Run();