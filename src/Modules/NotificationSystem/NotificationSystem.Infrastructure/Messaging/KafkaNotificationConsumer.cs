using System.Text.Json;
using System.Threading.Channels;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationSystem.Application.Commands.SendEmailNotification;
using NotificationSystem.Application.Commands.SendNotification;
using NotificationSystem.Application.Commands.SendPushNotification;
using NotificationSystem.Application.Commands.SendSmsNotification;
using Polly;
using Polly.Retry;
using Shared.Common.Notifications;

namespace NotificationSystem.Infrastructure.Messaging;

public class KafkaNotificationConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaNotificationConsumer> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;
    private readonly Channel<NotificationMessage> _messageChannel;
    private IConsumer<Ignore, string> _consumer;
    private Task _consumptionTask;

    public KafkaNotificationConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<KafkaNotificationConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _topic = configuration["KafkaNotificationTopic"] ?? "notifications";

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["KafkaBootstrapServers"] ?? "localhost:9092",
            GroupId = configuration["KafkaConsumerGroup"] ?? "notification-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = configuration["KafkaUsername"],
            SaslPassword = configuration["KafkaPassword"]
        };

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 60)),
                onRetry: (exception, timeSpan, context) =>
                {
                    _logger.LogWarning(exception, "Error occurred while consuming Kafka message. Retrying in {TimeSpan}...", timeSpan);
                });

        _messageChannel = Channel.CreateUnbounded<NotificationMessage>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Start the Kafka connection task
        _ = ConnectToKafkaAsync(stoppingToken);

        // Start processing messages from the channel
        return ProcessMessagesAsync(stoppingToken);
    }

    private async Task ConnectToKafkaAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Connecting to Kafka...");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
                    _consumer.Subscribe(_topic);
                    _logger.LogInformation("Kafka consumer created and subscribed to topic: {Topic}", _topic);

                    _consumptionTask = Task.Run(async () =>
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            try
                            {
                                _logger.LogInformation("Consuming Kafka message...");
                                var consumeResult = _consumer.Consume(stoppingToken);
                                _logger.LogInformation("Received Kafka message: {Message}", consumeResult.Message.Value);
                                var message = JsonSerializer.Deserialize<NotificationMessage>(consumeResult.Message.Value);
                                _logger.LogInformation("Received notification message: {Message}", message);
                                await _messageChannel.Writer.WriteAsync(message, stoppingToken);
                                _consumer.Commit(consumeResult);
                            }
                            catch (OperationCanceledException)
                            {
                                _logger.LogInformation("Kafka consumer stopped");
                                break;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error consuming Kafka message");
                            }
                        }
                    }, stoppingToken);

                    await _consumptionTask;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Kafka");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken stoppingToken)
    {
        while (await _messageChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            while (_messageChannel.Reader.TryRead(out var message))
            {
                try
                {
                    _logger.LogInformation("Processing notification message: {Message}", message);
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var command = CreateCommand(message);
                    await mediator.Send(command, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing notification message");
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _messageChannel.Writer.Complete();
        _consumer?.Close();
        _consumer?.Dispose();
        if (_consumptionTask != null)
        {
            await _consumptionTask;
        }
        await base.StopAsync(stoppingToken);
    }

    private void DisposeConsumer()
    {
        try
        {
            _consumer?.Close();
            _consumer?.Dispose();
            _consumer = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing Kafka consumer");
        }
    }

    public override void Dispose()
    {
        DisposeConsumer();
        base.Dispose();
    }

    private SendNotificationCommand CreateCommand(NotificationMessage message)
    {
        return message.Type switch
        {
            "Email" => new SendEmailCommand
            {
                Recipient = message.Recipient,
                Subject = message.Subject,
                PlainTextContent = message.Content,
                HtmlContent = message.HtmlContent
            },
            "SMS" => new SendSmsCommand
            {
                Recipient = message.Recipient,
                Content = message.Content
            },
            "Push" => new SendPushNotificationCommand
            {
                Recipient = message.Recipient,
                Title = message.Subject,
                Body = message.Content,
                Data = (Dictionary<string, string>)message.Data
            },
            _ => throw new ArgumentException($"Unknown notification type: {message.Type}")
        };
    }
}