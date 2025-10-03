namespace Application.Interfaces;

public interface IOutboxWriter
{
    void Enqueue<T>(T message, string eventType);
}