using System.Text.Json;
using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Shared.Common.Notifications;

namespace NotificationSystem.Infrastructure.Messaging;

public class KafkaNotificationSender : INotificationSender, IHostedService, IDisposable
{
    private readonly ILogger<KafkaNotificationSender> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly Channel<NotificationMessage> _messageChannel;
    private readonly string _topic;
    private readonly ProducerConfig _producerConfig;
    private IProducer<Null, string> _producer;
    private Task _sendingTask;
    private CancellationTokenSource _cts;

    public KafkaNotificationSender(IConfiguration configuration, ILogger<KafkaNotificationSender> logger)
    {
        _logger = logger;
        _topic = configuration["KafkaNotificationTopic"] ?? "notifications";
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration["KafkaBootstrapServers"] ?? "localhost:9092"
        };

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Error occurred while sending Kafka message. Retry attempt {RetryCount} in {TimeSpan}...", retryCount, timeSpan);
                });

        _messageChannel = Channel.CreateUnbounded<NotificationMessage>();
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _sendingTask = SendMessagesAsync(_cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _messageChannel.Writer.Complete();
        if (_sendingTask != null)
        {
            await _sendingTask;
        }
        _producer?.Dispose();
    }

    public Task SendEmailAsync(string recipient, string subject, string plainTextContent, string htmlContent)
    {
        var message = new NotificationMessage
        {
            Type = "Email",
            Recipient = recipient,
            Subject = subject,
            Content = plainTextContent,
            HtmlContent = htmlContent
        };
        return _messageChannel.Writer.WriteAsync(message).AsTask();
    }

    public Task SendSmsAsync(string recipient, string content)
    {
        var message = new NotificationMessage
        {
            Type = "SMS",
            Recipient = recipient,
            Content = content
        };
        return _messageChannel.Writer.WriteAsync(message).AsTask();
    }

    public Task SendPushNotificationAsync(string recipient, string title, string body, IDictionary<string, string> data = null)
    {
        var message = new NotificationMessage
        {
            Type = "Push",
            Recipient = recipient,
            Subject = title,
            Content = body,
            Data = data
        };
        return _messageChannel.Writer.WriteAsync(message).AsTask();
    }

    private async Task SendMessagesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await EnsureProducerCreatedAsync();

                while (await _messageChannel.Reader.WaitToReadAsync(cancellationToken))
                {
                    while (_messageChannel.Reader.TryRead(out var message))
                    {
                        var serializedMessage = JsonSerializer.Serialize(message);
                        await _retryPolicy.ExecuteAsync(async () =>
                        {
                            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = serializedMessage }, cancellationToken);
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessagesAsync");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task EnsureProducerCreatedAsync()
    {
        if (_producer == null)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                _producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
                _logger.LogInformation("Kafka producer created");
                await Task.CompletedTask; // This is just to make the method async
            });
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _producer?.Dispose();
        _cts.Dispose();
    }
}