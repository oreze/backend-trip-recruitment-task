using BackendTripRecruitmentTask.API.Middlewares;
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