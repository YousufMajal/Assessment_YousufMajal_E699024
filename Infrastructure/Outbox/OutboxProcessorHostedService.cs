using Application.Events;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Aws;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Outbox;

// Shell Service implementing Outbox Pattern to ensure Transactional Integrity, Resilience to external Failures, At-Least-Once Delivery Guarantee.

public sealed class OutboxProcessorHostedService : BackgroundService
{
    private readonly ILogger<OutboxProcessorHostedService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly OutboxProcessorOptions _options;

    public OutboxProcessorHostedService(
        ILogger<OutboxProcessorHostedService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<OutboxProcessorOptions> options)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
    }

    // background processing loop that orchestrates the complete outbox processing:
    // 1. Creates service scope for database access
    // 2. Retrieves batch of unprocessed messages
    // 3. Processes each message
    // 4. Updates message status (processed/failed)
    // 5. Repeats on interval until cancellation requested
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Outbox processor hosted service started.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<Application.Interfaces.IUnitOfWork>();
                var snsService = scope.ServiceProvider.GetRequiredService<AwsSnsService>();

                var unprocessedMessages = await GetUnprocessedMessagesAsync(outboxRepository);
                if (!unprocessedMessages.Any())
                {
                    _logger.LogDebug("No unprocessed outbox messages found.");
                }
                else
                {
                    _logger.LogInformation("Processing {Count} outbox messages", unprocessedMessages.Count);

                    foreach (var message in unprocessedMessages)
                    {
                        try
                        {
                            _logger.LogInformation("Processing message {MessageId} of type {MessageType}", message.Id, message.Type);

                            switch (message.Type)
                            {
                                case "banking.withdrawal.performed.v1":
                                    await PublishWithdrawalEventToSns(message, snsService, cancellationToken);
                                    break;

                                default:
                                    _logger.LogWarning("Unknown message type: {MessageType} for message {MessageId}",
                                        message.Type, message.Id);
                                    throw new InvalidOperationException($"Unsupported message type: {message.Type}");
                            }

                            await outboxRepository.MarkAsProcessedAsync(message.Id, DateTime.UtcNow);
                            await unitOfWork.SaveChangesAsync(cancellationToken);

                            _logger.LogDebug("Successfully processed outbox message {MessageId} of type {MessageType}",
                                message.Id, message.Type);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to process outbox message {MessageId} of type {MessageType}",
                                message.Id, message.Type);

                            // Attempt to mark as failed
                            await MarkMessageAsFailedSafely(message.Id, ex.Message, outboxRepository, unitOfWork);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error occurred in outbox processing cycle");
            }

            await Task.Delay(_options.ProcessingInterval, cancellationToken);
        }

        _logger.LogInformation("Outbox processor hosted service stopped.");
    }

    // gets batch of unprocessed outbox messages
    private async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(IOutboxRepository outboxRepository)
    {
        var unprocessedMessages = await outboxRepository.GetUnprocessedMessagesAsync(_options.BatchSize);
        return unprocessedMessages.ToList();
    }

    /// Safely marks a message as failed
    private async Task MarkMessageAsFailedSafely(
        Guid messageId,
        string errorMessage,
        IOutboxRepository outboxRepository,
        Application.Interfaces.IUnitOfWork unitOfWork)
    {
        try
        {
            await outboxRepository.MarkAsFailedAsync(messageId, errorMessage);
            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception markFailedEx)
        {
            _logger.LogError(markFailedEx, "Critical: Failed to mark message {MessageId} as failed. Message may be reprocessed.", messageId);
        }
    }

    /// Deserializes withdrawal event, then publishes it to SNS.
    private async Task PublishWithdrawalEventToSns(OutboxMessage message, AwsSnsService snsService, CancellationToken cancellationToken)
    {
        var withdrawalEvent = JsonSerializer.Deserialize<WithdrawalPerformedIntegrationEvent>(message.Content);

        if (withdrawalEvent is null)
        {
            throw new InvalidOperationException($"Failed to deserialize withdrawal event from message {message.Id}");
        }

        var withdrawalDto = new Application.DTOs.WithdrawalEventDto(
            AccountId: withdrawalEvent.AccountId,
            Amount: withdrawalEvent.Amount,
            PreviousBalance: withdrawalEvent.PreviousBalance,
            NewBalance: withdrawalEvent.NewBalance,
            Timestamp: withdrawalEvent.OccurredUtc);

        var snsEvent = snsService.CreateWithdrawalEvent(withdrawalDto);
        await snsService.PublishEventAsync(snsEvent, cancellationToken);
    }
}