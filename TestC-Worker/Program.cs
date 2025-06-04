using TestC_contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDb();
builder.Services.AddTestCConnection();

var app = builder.Build();

app.Run();