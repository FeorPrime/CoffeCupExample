using contracts;
using worker1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDb();
builder.Services.AddTransport();

var app = builder.Build();

Console.WriteLine("worker started as: {0}", WorkerMeta.WorkerId.ToString());

app.Run();