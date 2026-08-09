using MediatR;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Exceptions;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Identity.Application.Handlers;

internal sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IEventBus eventBus
) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            throw new NotFoundException($"User with ID {request.UserId} not found.");
        }

        // 1. Guardrail: Check across bounded contexts if the user is eligible for deletion
        var eligibilityContext = new UserDeletionEligibilityContext();
        var eligibilityEvent = new UserDeletionEligibilityIntegrationEvent(
            user.Id, 
            user.Role.ToString(), 
            eligibilityContext);

        // This will be handled synchronously in-process by Bookings/Properties modules
        await eventBus.PublishAsync(eligibilityEvent, cancellationToken);

        if (!eligibilityContext.IsEligible)
        {
            var reasons = string.Join(", ", eligibilityContext.Reasons);
            throw new BadRequestException($"Cannot delete account: {reasons}");
        }

        // 2. Anonymize user data (Soft Delete)
        user.Delete();

        // 3. Save changes
        userRepository.Update(user);
        
        return Result.Success();
    }
}
