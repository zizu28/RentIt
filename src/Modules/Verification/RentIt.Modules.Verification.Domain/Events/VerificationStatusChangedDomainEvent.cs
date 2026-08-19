using RentIt.Modules.Verification.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Verification.Domain.Events;

public record VerificationStatusChangedDomainEvent(
    Guid VerificationId,
    VerificationStatus OldStatus,
    VerificationStatus NewStatus) : DomainEvent;
