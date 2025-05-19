using CoffeeCup.Contracts;
using CoffeeCup.Storage.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDb();
builder.Services.AddTransport();
builder.Services.AddScoped<StorageService>();
builder.Services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
var app = builder.Build();

app.Run();