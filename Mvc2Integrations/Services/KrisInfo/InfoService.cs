using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace Mvc2Integrations.Services.KrisInfo
{
    public class InfoService : IInfoService
    {
        private readonly KrisInfoSettings _settings;

        public InfoService(IOptions<KrisInfoSettings> settings)
        {
            _settings = settings.Value;
        }
        public List<KrisInfoServiceModel> GetKrisInfo()
        {
            var httpClient = new HttpClient();
            var s = httpClient.GetStringAsync(_settings.Url).Result;

            XDocument xml = XDocument.Parse(s);


            return  xml.Root.Descendants().Where(r => r.Name.LocalName == "entry")
                .Select(e => new KrisInfoServiceModel
                {
                    Id = e.Elements().FirstOrDefault(i => i.Name.LocalName == "id").Value,
                    Summary = e.Elements().FirstOrDefault(i => i.Name.LocalName == "summary").Value,
                    Title = e.Elements().FirstOrDefault(i => i.Name.LocalName == "title").Value,
                }).ToList();


        }
    }
}