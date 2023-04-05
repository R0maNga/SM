﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BLL.Services
{
    public class BackgroundConsumerForGameCheck : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IGamesHostedService _gameService;

        public BackgroundConsumerForGameCheck(IGamesHostedService gameService)
        {
            _gameService = gameService;
            Init();
        }

        private void Init()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "GameStorage",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);



        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _gameService.GameEvent(message, stoppingToken);
                Console.WriteLine("Received message: {0}", message);
            };

            _channel.BasicConsume(queue: "GameStorage",
                autoAck: true,
                consumer: consumer);


            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            base.Dispose();
        }
    }
}
