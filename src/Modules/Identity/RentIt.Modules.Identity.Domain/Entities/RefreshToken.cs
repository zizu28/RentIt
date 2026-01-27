using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Entities;

/// <summary>
/// Refresh token entity for JWT token refresh
/// </summary>
public sealed class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;

    private RefreshToken() { } // EF Core

    private RefreshToken(Guid id, Guid userId, string token, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(Guid userId, string token, TimeSpan expiresIn)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        return new RefreshToken(
            Guid.NewGuid(),
            userId,
            token,
            DateTime.UtcNow.Add(expiresIn)
        );
    }

    public void Revoke()
    {
        if (RevokedAt.HasValue)
            throw new InvalidOperationException("Token is already revoked");

        RevokedAt = DateTime.UtcNow;
    }
}
