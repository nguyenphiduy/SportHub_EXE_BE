using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetListUser();
    Task<IEnumerable<User>> GetListUserWithVenue();
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    void Update(User user);
    void Remove(User user);
    Task<List<User>> GetUsersByRoleAsync(string role, CancellationToken ct = default);
    Task<List<User>> GetUsersByVenueAsync(Guid venueId, CancellationToken ct = default);
    void UpdateV1(User user);
    Task<User?> GetByIdWithoutDecryptAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailWithoutDecryptAsync(string email, CancellationToken ct = default);
    void UpdatePasswordOnly(User user);
    void Deactivate(User user);
    void Activate(User user);
}
