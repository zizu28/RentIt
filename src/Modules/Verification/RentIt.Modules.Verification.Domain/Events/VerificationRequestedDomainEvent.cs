using RentIt.Modules.Verification.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Verification.Domain.Events;

public record VerificationRequestedDomainEvent(
    Guid VerificationId,
    Guid HostId,
    DocumentType DocumentType) : DomainEvent;
