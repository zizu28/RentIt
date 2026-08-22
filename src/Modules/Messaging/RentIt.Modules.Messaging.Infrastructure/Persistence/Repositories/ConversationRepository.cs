using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;

namespace RentIt.Modules.Messaging.Infrastructure.Persistence.Repositories;

internal sealed class ConversationRepository(MessagingDbContext dbContext) : IConversationRepository
{
    private readonly MessagingDbContext _dbContext = dbContext;

    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Conversation?> GetByParticipantsAsync(Guid participant1Id, Guid participant2Id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .FirstOrDefaultAsync(c => 
                (c.Participant1Id == participant1Id && c.Participant2Id == participant2Id) ||
                (c.Participant1Id == participant2Id && c.Participant2Id == participant1Id), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Conversations.AddAsync(conversation, cancellationToken);
    }

    public void Update(Conversation conversation)
    {
        _dbContext.Conversations.Update(conversation);
    }
}
