using DotNetEnv;
using Project.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
if(File.Exists("../.env")) Env.Load("../.env");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddDatabase();
builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
