
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using CoreService.Models.Cache;
using CoreService.Helpers.Cache;
using CoreService.Models.DTO;
using AutoMapper;

namespace CoreService.Integrations.AMQP.RabbitMQ.Consumer
{
    public class UserBansConsumer (IConfiguration configuration,
        IServiceProvider serviceProvider) : RabbitMqConsumer
    {
        protected string _queue = configuration["Integrations:AMQP:Rabbit:Exchange:UserBansExchange:Queues:CoreService"];
        protected string _exchange = configuration["Integrations:AMQP:Rabbit:Exchange:UserBansExchange:Name"];
        protected override async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result = JsonSerializer.Deserialize<UserDTO>(message);

                if (result != null)
                {
                    Console.WriteLine(message);

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var userCache = scope.ServiceProvider.GetRequiredService<IUserParametersCache>();
                        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                        
                        userCache.InsertUserParametersIntoCache(
                            userLogin: result.Phone,
                            userParameters: mapper.Map<UserParametersCacheEntry>(result)
                            );
                    }
                }

                if (_channel != null)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
}
