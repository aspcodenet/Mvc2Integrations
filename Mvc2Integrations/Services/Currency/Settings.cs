using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc2Integrations.Services.Currency
{
    public class Settings
    {
        public int Retries { get; set; }
        public string ApiKey { get; set; }
        public string ApiHost { get; set; }
    }
}
