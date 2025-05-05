using CoreService.Helpers;
using CoreService.Integrations.AMQP.RabbitMQ.Producer;
using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

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
        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return text;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            var result = new OperationResultLogDTO();
            result.Id = Guid.NewGuid();
            result.TraceId = Guid.NewGuid();
            result.IsSuccessful = true;
            if (context.Request.Headers.TryGetValue("TraceId", out var givenTraceId))
            {
                if (Guid.TryParse(givenTraceId, out var givenTraceGuid))
                {
                    result.TraceId = givenTraceGuid;
                }
            }
            context.Request.Headers["TraceId"] = result.TraceId.ToString();
            result.OperationStart = DateTime.UtcNow;
            try
            {

                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    await responseBody.FlushAsync();
                    responseBody.Seek(0, SeekOrigin.Begin);

                    result.ErrorMessage = await FormatResponse(context.Response);
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
            }
            finally
            {
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                result.StatusCode = context.Response.StatusCode;
                if (result.StatusCode >= 400)
                {
                    result.IsSuccessful = false;
                }
                else
                {
                    result.ErrorMessage = null;
                }
                result.ExecutionTime = (int)Math.Round(Math.Max(0, sw.ElapsedMilliseconds / 1000.0));
                result.OperationName = context.Request.Path;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var rabbitProducer = scope.ServiceProvider.GetRequiredService<IRabbitMqProducer>();
                    await rabbitProducer.SendOperationResultLogMessage(result);
                }
            }
        }
    }
}