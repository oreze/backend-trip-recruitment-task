using BackendTripRecruitmentTask.API.Middlewares;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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