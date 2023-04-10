﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BLL.Services
{
    public class MessageProducer:IMessageProducer
    {
        public void SendMessage<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory { HostName = "localhost" }; 
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);

            Console.WriteLine("Sent message {0}", message);
        }
    }
}
