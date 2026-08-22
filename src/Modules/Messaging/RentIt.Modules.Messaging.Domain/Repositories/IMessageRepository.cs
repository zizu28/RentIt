using RentIt.Modules.Messaging.Domain.Entities;

namespace RentIt.Modules.Messaging.Domain.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Message>> GetConversationMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
    void Update(Message message);
}
