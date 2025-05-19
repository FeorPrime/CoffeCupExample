using CoffeeCup.Api;
using CoffeeCup.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransport();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

//app.UseMiddleware<ValidationMappingMiddleware>();
app.UseExceptionHandler(ConfigureExceptionsHandler);
app.MapControllers();

app.Run();
return;

void ConfigureExceptionsHandler(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (ex is ValidationException validationException)
        {
            var problems = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                {
                    "ValidationErrors", validationException.Errors.Select(e => e.ErrorMessage).ToArray()
                }
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Errors",
            };
        } else
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = ex?.Message
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    });
}