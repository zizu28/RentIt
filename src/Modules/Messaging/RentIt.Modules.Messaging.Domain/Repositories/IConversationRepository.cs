using RentIt.Modules.Messaging.Domain.Entities;

namespace RentIt.Modules.Messaging.Domain.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByParticipantsAsync(Guid participant1Id, Guid participant2Id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    void Update(Conversation conversation);
}
