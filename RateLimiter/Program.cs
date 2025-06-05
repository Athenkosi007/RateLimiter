using System;
using System.Collections.Generic;

public class RateLimiter
{
    private readonly int _limit;
    private readonly TimeSpan _timeWindow;
    private readonly Dictionary<string, Queue<DateTime>> _userAccessLog;

    public RateLimiter(int limit, TimeSpan timeWindow)
    {
        _limit = limit;
        _timeWindow = timeWindow;
        _userAccessLog = new Dictionary<string, Queue<DateTime>>();
    }

    public bool AllowRequest(string userId)
    {
        if (!_userAccessLog.ContainsKey(userId))
            _userAccessLog[userId] = new Queue<DateTime>();

        var accessLog = _userAccessLog[userId];
        var now = DateTime.UtcNow;

        // Remove outdated entries
        while (accessLog.Count > 0 && now - accessLog.Peek() > _timeWindow)
            accessLog.Dequeue();

        if (accessLog.Count < _limit)
        {
            accessLog.Enqueue(now);
            return true;
        }

        return false;
    }
}

// Example usage
public class Program
{
    public static void Main()
    {
        var limiter = new RateLimiter(3, TimeSpan.FromSeconds(10));
        string userId = "user123";

        for (int i = 0; i < 5; i++)
        {
            bool allowed = limiter.AllowRequest(userId);
            Console.WriteLine($"Request {i + 1}: {(allowed ? "Allowed" : "Blocked")}");
            System.Threading.Thread.Sleep(2000); // simulate time between requests
        }
    }
}
