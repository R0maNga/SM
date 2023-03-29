﻿using System.ComponentModel;
using System.Text;
using BLL.Models.Input.OrderInput;
using BLL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace OrderService
{
    public class Test: BackgroundService
    {
        private  IConnection _connection;
        private  IModel _channel;
        private readonly IOrderHostedService _eventProcessor;



        public Test(IOrderHostedService eventProcessor)
        {
            _eventProcessor = eventProcessor;
            
            

            Init();
        }   
        private void  Init()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received +=  (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                // delete
                var Order  = JsonConvert.DeserializeObject<CreateOrderInput>(message);
                _eventProcessor.ProcessEvent(message, stoppingToken);
               // await _dbContext.AddAsync(Order);
                //await _dbContext.SaveChangesAsync(stoppingToken);
               //_channel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("Received message: {0}", message);
                Console.WriteLine(Order.Price.ToString(), Order.BasketId);
                
            };
            
            _channel.BasicConsume(queue: "hello",
                autoAck: true,
                consumer: consumer);

            //await _orderService.Create(Order, stoppingToken);
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
