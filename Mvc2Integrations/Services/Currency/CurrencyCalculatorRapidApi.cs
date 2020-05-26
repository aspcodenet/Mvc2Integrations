using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RestSharp;

namespace Mvc2Integrations.Services.Currency
{
    class CachedCurrencyCalculator : ICurrencyCalculator
    {
        private readonly ICurrencyCalculator _inner;
        private readonly IMemoryCache _cache;

        public CachedCurrencyCalculator(ICurrencyCalculator inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public List<string> GetCurrencies()
        {
            var ret =  _cache.GetOrCreate("GetCurrencies", entry => _inner.GetCurrencies());
            if(!ret.Any())
                _cache.Remove("GetCurrencies");
            return ret;
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            return _inner.ConvertCurrency(from, to, belopp);
        }
    }

    class RetryCurrencyCalculator : ICurrencyCalculator
    {
        private readonly ICurrencyCalculator _inner;
        private readonly Settings _settings;
        public RetryCurrencyCalculator(ICurrencyCalculator inner, IOptions<Settings> settings)
        {
            _settings = settings.Value;
            _inner = inner;
        }


        public List<string> GetCurrencies()
        {
            var result = new List<string>();

            Polly.Policy.HandleResult<List<string>>(r => r.Count() == 0)
                .Retry(_settings.Retries, onRetry: (result, i) => System.Threading.Thread.Sleep(i * 10000 + 5000)).Execute(() =>
                    {
                        result = _inner.GetCurrencies();
                        return result;
                    });
            return result;
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            var result = 0m;
            Polly.Policy.HandleResult<decimal>(r => r == 0)
                .Retry(_settings.Retries, onRetry: (result, i) => System.Threading.Thread.Sleep(i * 10000 + 5000)).Execute(() =>
                {
                    result = _inner.ConvertCurrency(from, to, belopp);
                    return result;
                });
            return result;
        }
    }


    class CurrencyCalculatorRapidApi : ICurrencyCalculator
    {
        private readonly Settings _settings;
        public CurrencyCalculatorRapidApi(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }
        public List<string> GetCurrencies()
        {
            var list = new List<string>();
            var client = new RestClient();
            var request = new RestRequest("https://currency-exchange.p.rapidapi.com/listquotes",
                Method.GET, DataFormat.Json);

            SetApiHeaders(request);
            var response = client.Execute(request);       


            if (response.StatusCode == HttpStatusCode.OK)
            {
                list = JsonConvert.DeserializeObject<List<string>>(response.Content);
            }
            return list;
        }

        private void SetApiHeaders(RestRequest request)
        {
            request.AddHeader("x-rapidapi-host", _settings.ApiHost);
            request.AddHeader("x-rapidapi-key", _settings.ApiKey);
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            var b = $"{belopp:0.0}".Replace(",", ".");
            var client = new RestClient($"https://currency-exchange.p.rapidapi.com/exchange?q={b}&from={from}&to={to}");
            var request = new RestRequest(Method.GET);
            SetApiHeaders(request);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Convert.ToDecimal(response.Content);
            }

            return 0;
        }
    }
}