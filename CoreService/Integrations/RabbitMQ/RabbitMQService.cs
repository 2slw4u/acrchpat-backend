using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Channels;

namespace CoreService.Integrations.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {

        IConnectionFactory _conenctionFactory = null;
        IConnection _connection = null;
        IChannel _channel = null;

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            this.SendMessage(message);
        }

        public async void SendMessage(string message)
        {
            if (this._conenctionFactory == null)
            {
                this._conenctionFactory = new ConnectionFactory()
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672")
                };
            }
            if (this._connection == null)
            {
                this._connection = await this._conenctionFactory.CreateConnectionAsync();
            }
            if (this._channel == null)
            {
                this._channel = await this._connection.CreateChannelAsync();
                await this._channel.QueueDeclareAsync(queue: "TestQueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
            }

            var body = Encoding.UTF8.GetBytes(message);

            await this._channel.BasicPublishAsync(exchange: "", 
                routingKey: "TestQueue",
                body: body
                );
        }
    }
}
