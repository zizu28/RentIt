using System.Text.Json.Serialization;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Events;
using RentIt.Modules.Identity.Domain.ValueObjects;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Entities;

/// <summary>
/// User aggregate root
/// </summary>
public sealed class User : AggregateRoot<Guid>
{
#pragma warning disable
    private readonly List<RefreshToken> _refreshTokens = [];
    public Email Email { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public string? VerificationToken { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}".Trim();

    private User() { } // EF Core

    public User(Guid id, Email email, PhoneNumber phoneNumber, PasswordHash passwordHash, UserRole role)
    {
        Id = id;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        Role = role;
        Status = UserStatus.Active;
        IsEmailVerified = false;
        IsPhoneVerified = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(Email email, PhoneNumber phoneNumber, PasswordHash passwordHash, UserRole role)
    {
        if (passwordHash is null)
        {
            throw new ArgumentNullException(nameof(passwordHash), "Password hash cannot be null.");
        }          
        var user = new User(Guid.NewGuid(), email, phoneNumber, passwordHash, role);
        user.AddDomainEvent(
            new UserRegisteredEvent(user.Id, user.Email.Value, user.PhoneNumber.Value, user.Role.ToString()
        ));
        return user;
    }

    public void UpdateProfile(string? firstName, string? lastName)
    {
        FirstName = firstName?.Trim();
        LastName = lastName?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetVerificationToken(string token)
    {
        VerificationToken = token;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail(string token)
    {
        if (IsEmailVerified) throw new InvalidOperationException("Email is already verified");
        if (VerificationToken != token) throw new InvalidOperationException("Invalid verification token");
        
        IsEmailVerified = true;
        VerificationToken = null;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new EmailVerifiedEvent(Id, Email.Value));
    }

    public void SetPasswordResetToken(string token, TimeSpan expiresIn)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiresAt = DateTime.UtcNow.Add(expiresIn);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetPassword(string token, PasswordHash newPasswordHash)
    {
        if (PasswordResetToken != token) throw new InvalidOperationException("Invalid reset token");
        if (PasswordResetTokenExpiresAt < DateTime.UtcNow) throw new InvalidOperationException("Reset token expired");

        PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyPhoneNumber()
    {
        if (IsPhoneVerified)
        {
            throw new InvalidOperationException("Phone number is already verified");
        }
            
        IsPhoneVerified = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new PhoneNumberVerifiedEvent(Id, PhoneNumber.Value));
    }

    public void UpdatePassword(PasswordHash newPasswordHash)
    {
        if (newPasswordHash is null)
        {
            throw new ArgumentNullException(nameof(newPasswordHash), "Password hash cannot be null.");
        }
            
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public RefreshToken AddRefreshToken(string token, TimeSpan expiresIn)
    {
        var refreshToken = RefreshToken.Create(Id, token, expiresIn);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        if (refreshToken == null)
            throw new InvalidOperationException("Refresh token not found");

        refreshToken.Revoke();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke();
        }
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserLoggedInEvent(Id, Email.Value, DateTime.UtcNow));
    }

    public void Suspend()
    {
        if (Status == UserStatus.Suspended)
            throw new InvalidOperationException("User is already suspended");

        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new InvalidOperationException("User is already active");

        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        Status = UserStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
    }
}
