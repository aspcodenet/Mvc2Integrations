using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Mvc2Integrations.Services.KrisInfo
{
    class CachedInfoService : IInfoService
    {
        private readonly IInfoService _inner;
        private readonly IMemoryCache _cache;

        public CachedInfoService(IInfoService inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public List<KrisInfoServiceModel> GetKrisInfo()
        {
            return _cache.GetOrCreate("KrisInfoServiceModel", entry => _inner.GetKrisInfo());
        }
    }
}