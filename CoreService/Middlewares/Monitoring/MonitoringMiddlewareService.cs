using CoreService.Helpers;
using CoreService.Integrations.AMQP.RabbitMQ.Producer;
using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text;

namespace CoreService.Middlewares.Monitoring
{
    public class MonitoringMiddlewareService
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        public MonitoringMiddlewareService(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            var result = new OperationResultLogDTO();
            result.Id = Guid.NewGuid();
            result.TraceId = Guid.NewGuid();
            if (context.Request.Headers.TryGetValue("TraceId", out var givenTraceId))
            {
                if (Guid.TryParse(givenTraceId, out var givenTraceGuid))
                {
                    result.TraceId = givenTraceGuid;
                }
            }
            context.Request.Headers["TraceId"] = result.TraceId.ToString();
            result.OperationStart = DateTime.UtcNow;
            result.IsSuccessful = true;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"{(int)(sw.ElapsedMilliseconds * 1000)}, {result.ErrorMessage}");
                result.StatusCode = context.Response.StatusCode;
                if (result.StatusCode == 401) result.ErrorMessage = "401 Unauthorized - please provide a bearer token";
                if (result.StatusCode >= 400) result.IsSuccessful = false;
                result.ExecutionTime = (int)(sw.ElapsedMilliseconds * 1000);
                result.OperationName = $"{context.Request.Method}: {context.Request.Path}";
                using (var scope = _serviceProvider.CreateScope())
                {
                    var rabbitProducer = scope.ServiceProvider.GetRequiredService<IRabbitMqProducer>();
                    await rabbitProducer.SendOperationResultLogMessage(result);
                }
            }
        }
    }
}