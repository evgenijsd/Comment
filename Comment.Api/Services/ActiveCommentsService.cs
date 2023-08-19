using Comment.Api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Concurrent;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Comment.Api.Services
{
    public class ActiveCommentsService
    {
        private readonly ConcurrentDictionary<Guid, DataRequest> _pendingRequests = new ConcurrentDictionary<Guid, DataRequest>();

        public bool Add(Guid key, DataRequest data) => _pendingRequests.TryAdd(key, data);

        public bool Remove(Guid key) => _pendingRequests.TryRemove(key, out _);

        public bool UpdateActivity(Guid key, DataRequest data)
        {
            if (_pendingRequests.TryGetValue(key, out var active))
            {
                active.Id = data.Id;
                active.IsSaved = data.IsSaved;
                return true;
            }

            return false;
        }

        public int GetId(Guid key)
        {
            if (_pendingRequests.TryGetValue(key, out var active))
            {
                return active.Id;
            }

            return default;
        }

        public bool CheckKey(Guid key)
        {
            if (_pendingRequests.TryGetValue(key, out var active))
            {
                return true;
            }

            return false;
        }
    }
}
