using TestC_contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStateMachine();

var app = builder.Build();

app.Run();