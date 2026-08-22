using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;

namespace RentIt.Modules.Messaging.Infrastructure.Persistence.Repositories;

internal sealed class MessageRepository(MessagingDbContext dbContext) : IMessageRepository
{
    private readonly MessagingDbContext _dbContext = dbContext;

    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> GetConversationMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _dbContext.Messages.AddAsync(message, cancellationToken);
    }

    public void Update(Message message)
    {
        _dbContext.Messages.Update(message);
    }
}
