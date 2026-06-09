using System.Collections.Concurrent;

namespace BidaPlatform.Application.Services;

public class OnlineUserTracker
{
    // userId -> connectionIds
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _onlineUsers
        = new();

    public void UserConnected(Guid userId, string connectionId)
    {
        var connections = _onlineUsers.GetOrAdd(userId, _ => new HashSet<string>());
        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public void UserDisconnected(Guid userId, string connectionId)
    {
        if (!_onlineUsers.TryGetValue(userId, out var connections)) return;

        lock (connections)
        {
            connections.Remove(connectionId);
            if (connections.Count == 0)
            {
                _onlineUsers.TryRemove(userId, out _);
            }
        }
    }

    public bool IsOnline(Guid userId)
        => _onlineUsers.ContainsKey(userId);

    public IEnumerable<Guid> GetOnlineUserIds()
        => _onlineUsers.Keys;
}
