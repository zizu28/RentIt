using RentIt.Modules.Verification.Domain.Enums;
using RentIt.Modules.Verification.Domain.Events;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Verification.Domain.Entities;

public class HostKycVerification : AggregateRoot<Guid>
{
    public Guid HostId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    
    // This string contains the encrypted bytes of the document number
    public string EncryptedDocumentNumber { get; private set; }
    
    public VerificationStatus Status { get; private set; }
    
    public DateTime? VerificationDate { get; private set; }
    public string? Comments { get; private set; }

#pragma warning disable CS8618
    private HostKycVerification() { }
#pragma warning restore CS8618

    private HostKycVerification(
        Guid id, 
        Guid hostId, 
        DocumentType documentType, 
        string encryptedDocumentNumber) : base(id)
    {
        HostId = hostId;
        DocumentType = documentType;
        EncryptedDocumentNumber = encryptedDocumentNumber;
        Status = VerificationStatus.Pending;
    }

    public static HostKycVerification RequestVerification(
        Guid hostId, 
        DocumentType documentType, 
        string encryptedDocumentNumber)
    {
        var verification = new HostKycVerification(
            Guid.NewGuid(), 
            hostId, 
            documentType, 
            encryptedDocumentNumber);
            
        verification.AddDomainEvent(new VerificationRequestedDomainEvent(
            verification.Id, 
            verification.HostId, 
            verification.DocumentType));
            
        return verification;
    }

    public void Approve(string? comments)
    {
        if (Status != VerificationStatus.Approved)
        {
            var oldStatus = Status;
            Status = VerificationStatus.Approved;
            VerificationDate = DateTime.UtcNow;
            Comments = comments;
            
            AddDomainEvent(new VerificationStatusChangedDomainEvent(Id, oldStatus, Status));
        }
    }

    public void Reject(string? comments)
    {
        if (Status != VerificationStatus.Rejected)
        {
            var oldStatus = Status;
            Status = VerificationStatus.Rejected;
            VerificationDate = DateTime.UtcNow;
            Comments = comments;
            
            AddDomainEvent(new VerificationStatusChangedDomainEvent(Id, oldStatus, Status));
        }
    }
}
