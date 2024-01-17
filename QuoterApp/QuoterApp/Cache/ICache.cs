using System;

namespace QuoterApp.Cache
{
    public interface ICache
    {
        T Get<T>(string key);
        bool Contains(string key);
        void Add<T>(string key, T value);
        void Remove(string key);
        void SetExpirationTime(TimeSpan expirationTime);
    }
}
