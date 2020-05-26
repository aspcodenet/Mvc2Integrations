using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Mvc2Integrations.Services
{
    public class KrisInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

    }
    public interface IInfoService
    {
        public List<KrisInfo> GetKrisInfo();
    }

    class CachedInfoService : IInfoService
    {
        private readonly IInfoService _inner;
        private readonly IMemoryCache _cache;

        public CachedInfoService(IInfoService inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public List<KrisInfo> GetKrisInfo()
        {
            return _cache.GetOrCreate("KrisInfo", entry => _inner.GetKrisInfo());
        }
    }


    public class InfoService : IInfoService
    {
        public List<KrisInfo> GetKrisInfo()
        {
            var httpClient = new HttpClient();
            var s = httpClient.GetStringAsync("http://api.krisinformation.se/v1/feed?format=xml").Result;

            XDocument xml = XDocument.Parse(s);


            return  xml.Root.Descendants().Where(r => r.Name.LocalName == "entry")
                .Select(e => new KrisInfo
                {
                    Id = e.Elements().FirstOrDefault(i => i.Name.LocalName == "id").Value,
                    Summary = e.Elements().FirstOrDefault(i => i.Name.LocalName == "summary").Value,
                    Title = e.Elements().FirstOrDefault(i => i.Name.LocalName == "title").Value,
                }).ToList();


        }
    }
}