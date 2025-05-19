using CoffeeCup.Contracts;
using FluentValidation;

namespace CoffeeCup.Api;

public class ValidationMappingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (ValidationException ex)
        {
            httpContext.Response.StatusCode = 400;
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = ex.Errors.Select(x => new ValidationResponse
                {
                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage
                })
            };

            await httpContext.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}