using System.Reflection;
using BackendTripRecruitmentTask.API.Middlewares;
using BackendTripRecruitmentTask.Application.Services;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, lc) => lc
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<TripDbContext>(
    options => options.UseInMemoryDatabase("TripDb"));

// https://github.com/jbogard/MediatR/issues/984 - AppDomain.CurrentDomain.GetAssemblies() didn't load all assemblies, 
// eg. BackendTripRecruitmentTask.Application. This way it's loaded explicitly.
var assemblyWithHandlers = Assembly.Load("BackendTripRecruitmentTask.Application");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assemblyWithHandlers));

builder.Services.AddScoped<ITripService, TripService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<CustomExceptionHandlerMiddleware>();

await DbSeeder.EnsureDatabaseSeeded(app);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();