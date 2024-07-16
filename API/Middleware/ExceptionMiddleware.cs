using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        public ExceptionMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment){
            _requestDelegate = requestDelegate;
            _hostEnvironment = environment;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context){
            try{
                await _requestDelegate(context);

            }catch(Exception ex){
                _logger.LogError($"Error", ex,ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                var response = _hostEnvironment.IsDevelopment() 
                ? new ApiException(context.Response.StatusCode,ex.Message,ex.Message,ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode,ex.Message,ex.Message,"Internal Server Error");

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}