using HuloToys_Service.Controllers.CarRegistration.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using XTECH_FRONTEND.Models;

namespace XTECH_FRONTEND.Services
{
    public class WorkQueueClient
    {
        private readonly QueueSettingViewModel queue_setting;
        private readonly ConnectionFactory factory;
        private readonly IConfiguration _configuration;
        public WorkQueueClient(IConfiguration configuration)
        {
            _configuration = configuration;
            queue_setting = new QueueSettingViewModel()
            {
                host = _configuration["Queue:Host"],
                port = Convert.ToInt32(_configuration["Queue:Port"]),
                v_host = _configuration["Queue:V_Host"],
                username = _configuration["Queue:Username"],
                password = _configuration["Queue:Password"],
            };
            factory = new ConnectionFactory()
            {
                HostName = queue_setting.host,
                UserName = queue_setting.username,
                Password = queue_setting.password,
                VirtualHost = queue_setting.v_host,
                Port = Protocols.DefaultProtocol.DefaultPort
            };
        }
        public bool SyncQueue(RegistrationRecord model)
        {
            try
            {
                //var j_param = model;
                var _data_push = JsonConvert.SerializeObject(model);
                // Push message vào queue
                var response_queue = InsertQueueSimpleDurable(_data_push, _configuration["Queue:QueueSyncES"]);

                return true;
            }
            catch (Exception ex)
            {


            }
            return false;
        }
        public bool InsertQueueSimple(string message, string queueName)
        {

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                    return true;

                }
                catch (Exception ex)
                {


                    return false;
                }
            }
        }
        public bool InsertQueueSimpleDurable(string message, string queueName)
        {

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                    return true;

                }
                catch (Exception ex)
                {

                    return false;
                }
            }
        }
        public bool SyncQueueTest(RegistrationRecord model)
        {
            try
            {
                //var j_param = model;
                var _data_push = JsonConvert.SerializeObject(model);
                // Push message vào queue
                var response_queue = InsertQueueSimpleDurable(_data_push, _configuration["Queue:QueueSyncTest"]);

                return true;
            }
            catch (Exception ex)
            {


            }
            return false;
        }
    }
}
